using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace TcpInteract
{
    /// <summary>
    /// Provides the base functionality for a <see cref="TcpInteract"/> TCP server.
    /// </summary>
    public abstract class ServerBase : IDisposable
    {
        private readonly Timer timerSolicitorChecker = new Timer();
        private readonly Timer timerTimedOutChecker = new Timer();
        private Socket serverSocket;
        private bool enabled;

        #region Properties
        /// <summary>
        /// Gets the content pusher notification system for the server.
        /// </summary>
        public ContentPusher Pusher { get; } = new ContentPusher();

        private string refusePattern;
        /// <summary>
        /// Gets or sets the pattern that when matched on a client name, will refuse the client.
        /// The default pattern will not allow users to login with certain chars.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public string RefusePattern
        {
            get { return refusePattern; }
            set
            {
                // Check pattern.
                Regex regex = new Regex(value);
                refusePattern = value;
            }
        }

        /// <summary>
        /// Gets or sets the send and receive buffer size for the server socket.
        /// </summary>
        public int BufferSize
        {
            get { return serverSocket.ReceiveBufferSize; }
            set
            {
                serverSocket.ReceiveBufferSize = value;
                serverSocket.SendBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the time to wait (in microseconds) for a response when polling a connection.
        /// </summary>
        public int PollWait { get; set; } = 300 * 1000; // 500 MS

        /// <summary>
        /// Gets or sets the time, in milliseconds, to wait between solicitor checks.
        /// </summary>
        public double SolicitorCheckInterval
        {
            get { return timerSolicitorChecker.Interval; }
            set { timerSolicitorChecker.Interval = value; }
        }

        /// <summary>
        /// Gets or sets the time, in milliseconds, to wait between checks for timed
        /// out clients.
        /// </summary>
        public double TimedOutCheckInterval
        {
            get { return timerTimedOutChecker.Interval; }
            set { timerTimedOutChecker.Interval = value; }
        }

        private int solicitorThreshold = 3000;
        /// <summary>
        /// Gets or sets the time, in milliseconds, in which a client has to be connected
        /// but not logged in to be considered a solicitor. Solicitors are disconnected from
        /// the server once identified. A value of 3000 means that a client can be connected for
        /// 3 seconds without logging in, before the server disconnects the client.
        /// </summary>
        public int SolicitorThreshold
        {
            get { return solicitorThreshold; }
            set
            {
                if (solicitorThreshold < 50)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The value must be greater than or equal to 50. "
                        + "Any value under 50 will not give clients ample time to login.");
                }
                solicitorThreshold = value;

            }
        }

        /// <summary>
        /// Gets the port number to listen on (1 - 65,535).
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets the clients currently connected to this server.
        /// </summary>
        private readonly List<ServerSideClient> clients = new List<ServerSideClient>();
        /// <summary>
        /// Gets a read-only list of all connected clients.
        /// </summary>
        protected IReadOnlyList<ServerSideClient> Clients => clients;

        private readonly BindingList<string> clientNames = new BindingList<string>();
        /// <summary>
        /// Gets a bindable, read-only list of client names.
        /// </summary>
        public IReadOnlyList<string> ClientNames => clientNames;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerBase"/> class
        /// with the specified argument.
        /// </summary>
        /// <param name="port">The port to bind to.</param>
        /// <param name="syncContext">The synchronization context to use for marshaling calls onto the main thread.</param>
        protected ServerBase(int port, SynchronizationContext syncContext = null)
        {
            UiContext.Default.Initialize(syncContext);

            // Initial refuse pattern will disallow certain chars.
            if (port < 0 || port > 65535)
                throw new ArgumentException($@"The value of ""{port}"" must be greater than or equal to 0, and less than 65536.");

            Port = port;
            timerSolicitorChecker.Elapsed += delegate { UiContext.Default.Invoke(CheckForSolicitors); };
            timerSolicitorChecker.Interval = 2000;

            timerTimedOutChecker.Elapsed += delegate  {   UiContext.Default.Invoke(CheckForTimedOut); };
            timerTimedOutChecker.Interval = 5000;
        }

        /// <summary>
        /// Raised when the server refuses the connection of a client.
        /// </summary>
        /// <param name="content">Describes why the client has been refused.</param>
        protected virtual void OnConnectionRefused(ConnectionRefusedContent content)
        {
            Pusher.Push(content);
        }

        /// <summary>
        /// Raised when a client logs out.
        /// </summary>
        /// <param name="content">Describes the reason why the client logged out.</param>
        protected virtual void OnClientLoggedOut(LogoutContent content)
        {
            Pusher.Push(content);
        }

        /// <summary>
        /// Raised when a client logs in.
        /// </summary>
        /// <param name="clientName">The name of the client that has logged in.</param>
        protected virtual void OnClientLoggedIn(string clientName)
        {
            Pusher.Push(new LoginContent(clientName));
        }

        private void OnPackageReceivedCore(ServerSideClient client, Package e)
        {
            switch ((BaseCommands)e.Command)
            {
                case BaseCommands.ClientNames: SendClientNames(client); break;

                case BaseCommands.Logout:
                    clients.Remove(client);
                    BroadcastPackageAsync(e, client);
                    clientNames.Remove(client.Name);
                    client.Dispose();
                    OnClientLoggedOut(LogoutContent.Deserialize(e.Content));
                    break;

                case BaseCommands.Login: ProcessLoginPackage(client, e); break;

                case BaseCommands.Sync:
                    Synchronize(client);
                    break;
            }

            OnPackageReceived(client, e);
        }

        /// <summary>
        /// When a client receives an entire package, this method is to be used to process it.
        /// </summary>
        /// 
        protected abstract void OnPackageReceived(ServerSideClient client, Package e);

        /// <summary>
        /// Looks for clients that are connected but not doing anything and removes them.
        /// </summary>
        private void CheckForSolicitors()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].ConnectionTime != null && clients[i].LoggedInTime == null)
                {
                    TimeSpan span = DateTime.Now - clients[i].ConnectionTime.Value;

                    // If client has connected, but hasn't logged in within 3 seconds
                    // then refuse the client.
                    if (span.TotalMilliseconds > solicitorThreshold)
                    {
                        const ConnectionRefusedReason REASON = ConnectionRefusedReason.NoLogin;
                        var content = new ConnectionRefusedContent(REASON, clients[i].Name);
                        clients[i].SendPackageAsync((int)BaseCommands.ConnectionRefused, content);
                        OnConnectionRefused(content);
                        clientNames.Remove(clients[i].Name);
                        clients[i].Dispose();
                        clients.RemoveAt(i);
                        i--;
                        if (clients.Count == 0) break;
                    }
                }
            }
        }

        /// <summary>
        /// Looks for, and removes timed out clients.
        /// </summary>
        private async void CheckForTimedOut()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                // 500 ms wait time.
                bool isInactive = await clients[i].IsInactiveTaskAsync(PollWait);

                if (isInactive)
                {
                    var args = new LogoutContent(clients[i].Name, LogoutReason.TimedOut);
                    UiContext.Default.Invoke(OnClientLoggedOut, args);
                    clients[i].Dispose();
                    clients.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Sends the names of all of the clients which are logged in to a specified client.
        /// </summary>
        /// <param name="client">The client to send the names to.</param>
        private void SendClientNames(ServerSideClient client)
        {
            var contents = new ClientNamesContent(clients.Select(c => c.Name).ToArray());
            client.SendPackageAsync((int)BaseCommands.ClientNames, contents);
        }

        /// <summary>
        /// Gets the key information about the specified client.
        /// </summary>
        /// <param name="clientName">The name of the client.</param>
        /// <returns>Null, if no client exists with the specified name.</returns>
        public ClientInfo GetClientInfo(string clientName)
        {
            return ClientInfo.FromClient(clients.Find(c => c.Name == clientName));
        }

        /// <summary>
        /// Sends synchronization data to the specified client.
        /// </summary>
        /// <param name="client">The client to send sync data to.</param>
        protected virtual void Synchronize(ServerSideClient client)
        {
            SendClientNames(client);
        }

        /// <summary>
        /// Checks the specified package for validity. If the name is valid, the method
        /// yield None, otherwise it will have other flags set indicating obscurities.
        /// </summary>
        private ConnectionRefusedReason GetRefuseReason(string clientName)
        {
            var reason = ConnectionRefusedReason.None;

            if (String.IsNullOrWhiteSpace(clientName))
                return ConnectionRefusedReason.EmptyName;

            // The name cannot match the refuse pattern and cannot be the same as the servers name
            if (!String.IsNullOrEmpty(refusePattern) && Regex.IsMatch(clientName, refusePattern))
                reason |= ConnectionRefusedReason.RegexInvalidated;

            if (clients.Any(c => clientName == c.Name))
                reason |= ConnectionRefusedReason.NameExists;

            return reason;
        }

        /// <summary>
        /// Starts listening for clients.
        /// </summary>
        /// <exception cref="SocketException">Raises when socket address is already in use.</exception>
        /// <param name="backlog">The maximum length of the pending connections queue.</param>
        public void Start(int backlog = 0)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            serverSocket.Listen(backlog);
            serverSocket.BeginAccept(AcceptCallback, null);
            timerSolicitorChecker.Start();
            timerTimedOutChecker.Start();
            enabled = true;
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public void Stop(string serverClosedMessage = "")
        {
            if (enabled)
            {
                timerSolicitorChecker.Stop();
                timerTimedOutChecker.Stop();
                var content = new ServerClosedContent(serverClosedMessage);
                Package package = new Package(BaseCommands.ServerClosed, content.Serialize());

                foreach (ServerSideClient client in clients)
                {
                    client.SendPackageAsync(package);
                    client.Socket.Shutdown(SocketShutdown.Both);
                    client.Socket.Close();
                }

                clients.Clear();
                clientNames.Clear();
                enabled = false;
                serverSocket.Close();

            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public Task StopTaskAsync(string serverClosedMessage = "")
        {
            return Task.Run(delegate { Stop(serverClosedMessage);});
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            if (enabled)
            {
                Socket socket = serverSocket.EndAccept(AR);

                UiContext.Default.Invoke(() =>
                {
                    ServerSideClient client = new ServerSideClient(socket);
                    clients.Add(client);
                    client.PackageReceived = OnPackageReceivedCore;
                    serverSocket.BeginAccept(AcceptCallback, null);
                });
            }
        }

        /// <summary>
        /// Analyzes the login package to see if it is a valid login. If it is, then login the client.
        /// </summary>
        private void ProcessLoginPackage(ServerSideClient client, Package package)
        {
            // The client does not have its name yet. Use the name provided by its login attempt.
            LoginContent loginContent = LoginContent.Deserialize(package.Content);
            var reason = GetRefuseReason(loginContent.ClientName);

            // Here I am not rejecting the user and we will send the package out for 
            // further processing.
            if (reason == ConnectionRefusedReason.None)
            {
                client.Name = loginContent.ClientName;
                client.Status = ClientStatus.LoggedIn;
                BroadcastPackageAsync(package);
                client.TimeStampLogin();
                clientNames.Add(client.Name);
                OnClientLoggedIn(client.Name);
            }
            else
            {
                var args = new ConnectionRefusedContent(reason, loginContent.ClientName);
                // Here I am rejecting the user, we will send the reject message to the appropriate client
                var pkgResp = new Package(BaseCommands.ConnectionRefused, args.Serialize());
                client.SendPackageAsync(pkgResp);
                clientNames.Remove(client.Name);
                clients.Remove(client);
                client.Socket.Shutdown(SocketShutdown.Both);
                client.Dispose();
                OnConnectionRefused(args);
            }
        }

        #region Broadcasting
        /// <summary>
        /// Sends the specified package to all connected clients except the specified one.
        /// </summary>
        /// <param name="package">The <see cref="Package"/> to send to the clients.</param>
        /// <param name="excludeClient">The client to exclude from the broadcast</param>
        /// <param name="mustBeLoggedIn">Whether the package is sent to all connected clients or 
        /// just the ones that are logged in.</param>
        protected void BroadcastPackage(Package package, ServerSideClient excludeClient, bool mustBeLoggedIn = true)
        {
            foreach (ServerSideClient client in clients)
            {
                if (mustBeLoggedIn && client.Status != ClientStatus.LoggedIn)
                    continue;

                if (client != excludeClient)
                    client.SendPackage(package);
            }
        }

        /// <summary>
        /// Sends the specified package to all connected clients.
        /// </summary>
        /// <param name="package">The <see cref="Package"/> to send to all of the clients.</param>
        /// <param name="mustBeLoggedIn">Whether the package is sent to all connected clients or 
        /// just the ones that are logged in.</param>
        protected void BroadcastPackage(Package package, bool mustBeLoggedIn = true)
        {
            foreach (ServerSideClient client in clients)
            {
                if (mustBeLoggedIn && client.Status != ClientStatus.LoggedIn)
                    continue;

                client.SendPackage(package);
            }
        }

        /// <summary>
        /// Sends the specified package to all connected clients asynchronously.
        /// </summary>
        /// <param name="package">The <see cref="Package"/> to send to all of the clients.</param>
        /// <param name="mustBeLoggedIn">Whether the package is sent to all connected clients or 
        /// just the ones that are logged in.</param>
        protected void BroadcastPackageAsync(Package package, bool mustBeLoggedIn = true)
        {
            foreach (ServerSideClient client in clients)
            {
                if (mustBeLoggedIn && client.Status != ClientStatus.LoggedIn)
                    continue;

                client.SendPackageAsync(package);
            }
        }

        /// <summary>
        /// Sends the specified package asynchronously to all connected clients except the specified one.
        /// </summary>
        /// <param name="package">The <see cref="Package"/> to send to the clients.</param>
        /// <param name="excludeClient">The client to exclude from the broadcast</param>
        /// <param name="mustBeLoggedIn">Whether the package is sent to all connected clients or 
        /// just the ones that are logged in.</param>
        protected void BroadcastPackageAsync(Package package, ServerSideClient excludeClient, bool mustBeLoggedIn = true)
        {
            foreach (ServerSideClient client in clients)
            {
                if (mustBeLoggedIn && client.Status != ClientStatus.LoggedIn)
                    continue;

                if (client != excludeClient)
                    client.SendPackageAsync(package);
            }
        }
        #endregion

        /// <summary>
        /// Boots the specified client from the server.
        /// </summary>
        /// <exception cref="ArgumentException">Client could not be found.</exception>
        public void KickClient(string name, string reason = "")
        {
            var client = clients.Find(c => c.Name == name);

            if (client == null)
            {
                throw new ArgumentException($@"Client ""{name}"" not found.", nameof(name));
            }

            var args = new LogoutContent(name, LogoutReason.Kicked, reason);
            var package = new Package((int)BaseCommands.Logout, args.Serialize());
            BroadcastPackage(package);
            clientNames.Remove(client.Name);
            client.Socket.Shutdown(SocketShutdown.Both);
            client.Dispose();
            clients.Remove(client);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ServerBase"/>.
        /// </summary>
        public virtual void Dispose()
        {
            Stop();
            timerSolicitorChecker.Dispose();
            timerTimedOutChecker.Dispose();
        }
    }
}
