using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TcpInteract
{
    /// <summary>
    /// Provides the base functionality for a <see cref="TcpInteract"/> TCP client.
    /// </summary>
    public abstract class ClientBase : IDisposable
    {
        // To make the client resuable, the Socket property is disposed re-instantiated when needed.
        // I could not depend on Socket reusability. Since the socket is recreated, the set buffer size
        // needs to be a field so it can persists across multiple reuses of the client.
        private readonly byte[] buffer = new byte[8192];
        private byte[] data = { };

        #region Properties
        /// <summary>
        /// Gets or sets whether the client should receive or send data. This is a code guard
        /// which guards against SocketExceptions when the client is disconnected. When true,
        /// send and receive operations will be terminated more quickly.
        /// </summary>
        protected internal bool ReceiveSendGuard { get; set; }

        private ClientStatus status;
        /// <summary>
        /// Gets the connection status of the client.
        /// </summary>
        public ClientStatus Status
        {
            get { return status; }
            set
            {
                if (value != status)
                {
                    status = value;
                    UiContext.Default.Invoke(OnStatusChanged);
                }
            }
        }

        /// <summary>
        /// Gets the handle of the client's socket.
        /// </summary>
        public IntPtr SocketHandle => Socket?.Handle ?? IntPtr.Zero;

        /// <summary>
        /// Gets or sets this client's socket used for small data transfers.
        /// </summary>
        internal protected Socket Socket { get; protected set; }

        private string name;
        /// <summary>
        /// Gets or sets the name of this client. This is also the clients identifier. Two clients
        /// with the same name cannot log into the same TcpInteract server. The name must be set before
        /// the client logs in.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot change client name while logged in.</exception>
        public string Name
        {
            get { return name; }
            set
            {
                if (Status == ClientStatus.LoggedIn)
                {
                    throw new InvalidOperationException(@"Cannot change client name while logged in.");
                }

                name = value;
            }
        }

        /// <summary>
        /// Gets the time in which this client has established a connection. 
        /// </summary>
        public DateTime? ConnectionTime { get; protected set; }

        /// <summary>
        /// Gets the time in which this client has logged in. 
        /// </summary>
        public DateTime? LoggedInTime { get; protected set; }
        #endregion

        /// <summary>
        /// Occurs when the value of the <see cref="Status"/> property has changed.
        /// </summary>
        public event EventHandler StatusChanged = delegate { };
        /// <summary>
        /// Raises the <see cref="StatusChanged"/> event.
        /// </summary>
        protected virtual void OnStatusChanged()
        {
            StatusChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises when a client logs out.
        /// </summary>
        /// <param name="content">Content that describes what client has logged out and for what reason.</param>
        protected virtual void OnLoggedOut(LogoutContent content) { }

        /// <summary>
        /// Raises when a list of client names has been received.
        /// </summary>
        /// <param name="content">Content that lists the names of all clients logged in.</param>
        protected virtual void OnNamesReceived(ClientNamesContent content) { }

        /// <summary>
        /// Raises when a client has logged in.
        /// </summary>
        /// <param name="content">Content to describe a successful client login.</param>
        protected virtual void OnLoggedIn(LoginContent content) { }

        /// <summary>
        /// Raises when a client received a completed <see cref="Package"/>.
        /// Not calling the base implementation of this methods will cause the client
        /// to malfunction.
        /// </summary>
        /// <param name="package">The package that has been received.</param>
        protected virtual void OnPackageReceived(Package package)
        {
            switch ((BaseCommands)package.Command)
            {
                case BaseCommands.Logout:
                    var content = LogoutContent.Deserialize(package.Content);
                    OnLoggedOut(content);
                    break;

                case BaseCommands.ClientNames:
                    var c = ClientNamesContent.Deserialize(package.Content);
                    OnNamesReceived(c);
                    break;

                case BaseCommands.Login:
                    var loginContent = LoginContent.Deserialize(package.Content);
                    OnLoggedIn(loginContent);
                    break;
            }
        }

        /// <summary>
        /// Raises when the client has disconnected ungracefully. This disconnect is detected
        /// only when the client sends or receives.
        /// </summary>
        protected virtual void OnClientTimedOut() { }

        /// <summary>
        /// Resets the client for reconnection. Implement any prestige logic here.
        /// </summary>
        protected virtual void ResetClient()
        {
            // Having the receive guard here may be problematic.
            // It may keep important packages from reaching the client as the client disconnects.
            // More likely larger packages.
            ReceiveSendGuard = true;
            Status = ClientStatus.Disconnected;
            Socket.Close();
            LoggedInTime = null;
            ConnectionTime = null;
        }

        /// <summary>
        /// Gets whether the client is actively connected asynchronously.
        /// </summary>
        /// <param name="waitTime">The wait time in microseconds.</param>
        public Task<bool> IsInactiveTaskAsync(int waitTime)
        {
            return Task.Run(() => IsInactive(waitTime));
        }

        /// <summary>
        /// Gets whether the client is actively connected.
        /// </summary>
        /// <param name="waitTime">The wait time in microseconds.</param>
        public bool IsInactive(int waitTime)
        {
            if (!Socket.Connected) return true;
            bool closed, noData;

            try
            {
                closed = Socket.Poll(waitTime, SelectMode.SelectRead);
                noData = Socket.Available == 0;
            }
            catch (SocketException)
            {
                return true;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }

            return closed && noData;
        }

        /// <summary>
        /// Processes the bytes received by this client.
        /// </summary>
        /// <param name="recBuffer">Only received bytes and no leftovers.</param>
        private void ProcessReceived(byte[] recBuffer)
        {
            data = data.Append(recBuffer); // Add buffer to the start of the array
            int packageLength;

            // If we have a package with bytes in it and enough data accumulated to complete a package
            // continue to loop until as many packages are extracted from data.
            while (data.Length > 4 && (packageLength = BitConverter.ToInt32(data, 0)) > 0
                && data.Length - 4 >= packageLength)
            {
                var dataPackage = new byte[packageLength];
                Buffer.BlockCopy(data, 4, dataPackage, 0, packageLength);
                data = data.TrimStart(packageLength + 4); // Remove package and length prefix.
                var package = new Package(BitConverter.ToInt32(dataPackage, 0), dataPackage.TrimStart(4));
                UiContext.Default.Invoke(OnPackageReceived, package);
            }
        }

        private void SendCallback(IAsyncResult AR)
        {
            try
            {
                if (Socket.Connected)
                {
                    Socket.EndSend(AR);
                }
            }
            catch (SocketException)
            {
                ResetClient();
                OnClientTimedOut();
                Debugger.Break();
            }
            catch (ObjectDisposedException)
            {
                ResetClient();
                OnClientTimedOut();
                Debugger.Break();
            }
        }

        #region Send Methods
        /// <summary>
        /// Sends the specified package asynchronously to the server.
        /// </summary>
        protected void SendPackageAsyncBase(int command)
        {
            SendPackageAsyncBase(new Package(command));
        }

        /// <summary>
        /// Sends the specified package asynchronously to the server.
        /// </summary>
        protected void SendPackageAsyncBase(Package package)
        {
            byte[] tempData = package.Serialize();

            try
            {
                if (Socket.Connected)
                {
                    Socket.BeginSend(tempData, 0, tempData.Length, SocketFlags.None, SendCallback, null);
                }
            }
            catch (SocketException)
            {
                ResetClient();
                OnClientTimedOut();
                Debugger.Break();
            }
            catch (ObjectDisposedException)
            {
                ResetClient();
                OnClientTimedOut();
                Debugger.Break();
            }
        }

        /// <summary>
        /// Sends the specified package asynchronously to the server.
        /// </summary>
        /// <returns>How many bytes were sent.</returns>
        protected Task<int> SendPackageTaskAsyncBase(Package package)
        {
            return Task.Run(() => SendPackageBase(package));
        }

        /// <summary>
        /// Sends the specified package asynchronously to the server.
        /// </summary>
        /// <returns>How many bytes were sent.</returns>
        protected Task<int> SendPackageTaskAsyncBase(int command, ISerializable serializable)
        {
            return Task.Run(() => SendPackageBase(command, serializable));
        }

        /// <summary>
        /// Sends the specified package asynchronously to the server.
        /// </summary>
        protected void SendPackageAsyncBase(int command, ISerializable serializable)
        {
            SendPackageAsyncBase(new Package(command, serializable.Serialize()));
        }

        /// <summary>
        /// Sends the package synchronously to the server.
        /// </summary>
        /// <returns>Returns the amount of data sent in bytes.</returns>
        protected int SendPackageBase(int command, ISerializable serializable)
        {
            return SendPackageBase(new Package(command, serializable.Serialize()));
        }

        /// <summary>
        /// Sends the package synchronously to the server.
        /// </summary>
        /// <returns>Returns the amount of data sent in bytes.</returns>
        protected int SendPackageBase(Package package)
        {
            if (Socket.Connected)
            {
                byte[] tempData = package.Serialize();

                try
                {
                    int sent = Socket.Send(tempData, 0, tempData.Length, SocketFlags.None);
                    return sent;
                }
                catch (SocketException)
                {
                    ResetClient();
                    OnClientTimedOut();
                    Debugger.Break();
                    return 0;
                }
                catch (ObjectDisposedException)
                {
                    ResetClient();
                    OnClientTimedOut();
                    Debugger.Break();
                    return 0;
                }
            }

            return -1;
        }
        #endregion

        /// <summary>
        /// Begins receiving data asynchronously.
        /// </summary>
        protected void StartReceiving()
        {
            ConnectionTime = DateTime.Now;
            Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                if (Socket.Connected && !ReceiveSendGuard)
                {
                    int received = Socket.EndReceive(AR);
                    // We will receive 0 when socket is disconnected semi-gracefully
                    // Like when we disconnect the client when refusing it (no logout msg sent).
                    if (received == 0)
                    {
                        ResetClient();
                        Debug.WriteLine("Receive loop exited (received == 0)");
                        return;
                    }

                    var data = new byte[received];
                    Buffer.BlockCopy(buffer, 0, data, 0, received);
                    ProcessReceived(data);

                    if (Socket.Connected && !ReceiveSendGuard)
                        Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
                }
            }
            catch (SocketException)
            {
                Debugger.Break();
                OnClientTimedOut();
                ResetClient();
            }
            catch (ObjectDisposedException)
            {
                OnClientTimedOut();
                ResetClient();
                Debugger.Break();
            }
        }

        #region Equality
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool Equals(ClientBase other)
        {
            return string.Equals(name, other.name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as ClientBase;
            return other != null && Equals(other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (name != null ? name.GetHashCode() : 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ClientBase left, ClientBase right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ClientBase left, ClientBase right)
        {
            return !Equals(left, right);
        }
        #endregion

        /// <summary>
        /// Releases all resources associated with this instance.
        /// </summary>
        public virtual void Dispose()
        {
            Socket?.Close();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return name;
        }
    }
}
