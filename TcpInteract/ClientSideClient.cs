using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TcpInteract
{
    /// <summary>
    /// Represents a TCP client to be used in client-side applications.
    /// </summary>
    public abstract class ClientSideClient : ClientBase
    {
        private bool abortConnectionPending;
        /// <summary>
        /// How many connection attempts have been made.
        /// </summary>
        private int connectionAttempts;

        #region Properties
        /// <summary>
        /// Gets the content pusher notification system for the client.
        /// </summary>
        public ContentPusher Pusher { get; } = new ContentPusher();

        /// <summary>
        /// Get or sets the endpoint to connect to.
        /// </summary>
        public IPEndPoint EndPoint { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of attempts before the ConnectionAborted event raises
        /// (0 is unlimited).
        /// </summary>
        public int MaxConnectionAttempts { get; set; }

        private readonly BindingList<string> clientNames = new BindingList<string>();
        /// <summary>
        /// Gets a bindable list of client names.
        /// </summary>
        public IReadOnlyList<string> ClientNames => clientNames;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSideClient"/> class with the specified
        /// optional argument.
        /// </summary>
        /// <param name="context">The synchronization context to use for thread marshaling.</param>
        protected ClientSideClient(SynchronizationContext context = null)
        {
            UiContext.Default.Initialize(context);
        }

        /// <summary>
        /// Raises when the server has refused the connection of the client.
        /// </summary>
        /// <param name="content">Describes why the server rejected the client.</param>
        protected virtual void OnConnectionRefused(ConnectionRefusedContent content)
        {
            Pusher.Push(content);
        }

        /// <summary>
        /// Occurs when a connection has been attempted and was failed.
        /// </summary>
        public event EventHandler<ConnectionAttemptFailedEventArgs> ConnectionAttemptFailed = delegate { };
        /// <summary>
        /// Raises the <see cref="ConnectionAttemptFailed"/> event.
        /// </summary>
        protected virtual void OnConnectionAttemptFailed(ConnectionAttemptFailedEventArgs e)
        {
            ConnectionAttemptFailed(this, e);
        }

        /// <summary>
        /// Raises when the server has closed.
        /// </summary>
        /// <param name="content">Content describing the occurrence.</param>
        protected virtual void OnServerClosed(ServerClosedContent content)
        {
            Pusher.Push(content);
        }

        /// <summary>
        /// Raises when a list of client names has been received.
        /// </summary>
        /// <param name="content">Content that lists the names of all clients logged in.</param>
        protected override void OnNamesReceived(ClientNamesContent content)
        {
            clientNames.Clear();

            foreach (var n in content.Names)
                clientNames.Add(n);

            Pusher.Push(content);
        }

        /// <summary>
        /// Raises when a client has logged in.
        /// </summary>
        /// <param name="content">Content to describe a successful client login.</param>
        protected override void OnLoggedIn(LoginContent content)
        {
            if (content.ClientName == Name)
            {
                Status = ClientStatus.LoggedIn;
                LoggedInTime = DateTime.Now;
            }

            clientNames.Add(content.ClientName);
            Pusher.Push(content);
        }

        /// <summary>
        /// Raises when a client logs out.
        /// </summary>
        /// <param name="content">Content that describes what client has logged out and for what reason.</param>
        protected override void OnLoggedOut(LogoutContent content)
        {
            clientNames.Remove(content.ClientName);
            Pusher.Push(content);

            if (content.Reason == LogoutReason.Kicked && content.ClientName == Name)
            {
                ResetClient();
            }
        }

        /// <summary>
        /// Raises when the client has disconnected ungracefully. This disconnect is detected
        /// only when the client sends or receives.
        /// </summary>
        protected override void OnClientTimedOut()
        {
            UiContext.Default.Invoke(OnLoggedOut, new LogoutContent(Name, LogoutReason.TimedOut));
        }

        /// <summary>
        /// Raises when a client received a completed <see cref="Package"/>.
        /// Not calling the base implementation of this methods will cause the client
        /// to malfunction.
        /// </summary>
        /// <param name="package">The package that has been received.</param>
        protected override void OnPackageReceived(Package package)
        {
            base.OnPackageReceived(package);

            switch ((BaseCommands)package.Command)
            {
                case BaseCommands.ConnectionRefused:
                
                    ResetClient();
                    var args = ConnectionRefusedContent.Deserialize(package.Content);
                    OnConnectionRefused(args);
                    break;

                case BaseCommands.ServerClosed:
                    var content = ServerClosedContent.Deserialize(package.Content);
                    ResetClient();
                    OnServerClosed(content);
                    break;
            }
        }

        /// <summary>
        /// Stops the connector from trying to connect. Does not guarantee abortion.
        /// </summary>
        public void AbortConnect()
        {
            abortConnectionPending = true;
        }

        /// <summary>
        /// Request synchronization data.
        /// </summary>
        public void Syncronize()
        {
            SendPackageAsyncBase((int)BaseCommands.Syncronize);
        }

        /// <summary>
        /// Sends a logout message to the server. There is no need to call <see cref="ClientBase.Dispose()"/>
        /// right after calling this method.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the client is not logged in.</exception>
        public void Logout()
        {
            Logout(LogoutReason.UserSpecified);
        }

        /// <summary>
        /// Sends a logout message to the server. There is no need to call <see cref="ClientBase.Dispose()"/>
        /// right after calling this method.
        /// </summary>
        private void Logout(LogoutReason reason)
        {
            if (Status == ClientStatus.LoggedIn)
            {
                var args = new LogoutContent(Name, reason);
                Package package = new Package(BaseCommands.Logout, args.Serialize());
                SendPackageAsyncBase(package);
            }

            if (Status != ClientStatus.Disconnected && Socket.Connected)
                Socket.Shutdown(SocketShutdown.Both);

            ResetClient();
        }

        /// <summary>
        /// Try to connect, according to the set <see cref="MaxConnectionAttempts"/>, asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="AlreadyLoggedInException"></exception>
        public void RequestLogin()
        {
            // Already requesting.
            if (Status == ClientStatus.Connected || Status == ClientStatus.Connecting)
                return;

            if (EndPoint == null)
                throw new InvalidOperationException($@"The property ""{EndPoint}"" cannot be null.");

            if (Status == ClientStatus.LoggedIn)
            {
                throw new AlreadyLoggedInException(
                    $@"""{Name}"" is already logged in. You must logout this client before logging in again.", null);
            }

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            abortConnectionPending = false;
            connectionAttempts = 0;
            ReceiveSendGuard = false;
            Status = ClientStatus.Connecting;

            Task.Run(() =>
            {
                while (Socket != null && !Socket.Connected && !abortConnectionPending)
                {
                    try
                    {
                        Socket.Connect(EndPoint);
                        Status = ClientStatus.Connected;
                        StartReceiving();
                        // Ask to login.
                        SendPackageAsyncBase((int)BaseCommands.Login, new LoginContent(Name));
                        return;
                    }
                    catch (SocketException)
                    {
                        connectionAttempts++;

                        var args = new ConnectionAttemptFailedEventArgs(
                            connectionAttempts,
                            MaxConnectionAttempts > 0 && connectionAttempts > MaxConnectionAttempts,
                            abortConnectionPending);

                        UiContext.Default.Invoke(delegate
                        {
                            if (args.EndOfAttempts)
                                Status = ClientStatus.Disconnected;

                            OnConnectionAttemptFailed(args);
                        });

                        if (args.EndOfAttempts)
                            break;
                    }
                }
            });
        }
    }
}