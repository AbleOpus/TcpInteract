using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TcpInteract
{
    /// <summary>
    /// Represents a TCP client to be used in server-side applications.
    /// </summary>
    public sealed class ServerSideClient : ClientBase
    {
        /// <summary>
        /// Handles the <see cref="PackageReceived"/> method and delegate.
        /// </summary>
        /// <param name="sender">This instance.</param>
        /// <param name="args">The completed package.</param>
        internal delegate void PackageReceivedHandler(ServerSideClient sender, Package args);

        // We are not using events here. The subscribed methods should fire immediately. This method
        // is not for external consumption but for server hooking so the server can quickly process
        // all of the packages received by each client.
        /// <summary>
        /// Occurs when a <see cref="Package"/> has been received.
        /// </summary>
        internal PackageReceivedHandler PackageReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBase"/> class as a server-side client.
        /// </summary>
        public ServerSideClient(Socket socket)
        {
            Socket = socket;
            StartReceiving();
            ConnectionTime = DateTime.Now;
        }

        /// <summary>
        /// Sets the <see cref="ClientBase.LoggedInTime"/> to the time this method is called.
        /// </summary>
        internal void TimeStampLogin()
        {
            LoggedInTime = DateTime.Now;
        }

        /// <summary>
        /// Raises when a client received a completed <see cref="Package"/>.
        /// </summary>
        /// <param name="package">The package that has been received.</param>
        protected override void OnPackageReceived(Package package)
        {
            base.OnPackageReceived(package);
            PackageReceived?.Invoke(this, package);
        }

        /// <summary>
        /// Raises when a client logs out.
        /// </summary>
        /// <param name="content">Content that describes what client has logged out and for what reason.</param>
        protected override void OnLoggedOut(LogoutContent content)
        {
            if (content.ClientName == Name)
            {
                Status = ClientStatus.Disconnected;
            }
        }

        // Expose protected members so the server can work with them.
        #region Send Methods
        /// <summary>
        /// Sends the specified package asynchronously to the server.
        /// </summary>
        public void SendPackageAsync(int command)
        {
            SendPackageAsyncBase(command);
        }

        /// <summary>
        /// Sends the specified package asynchronously to the server.
        /// </summary>
        public void SendPackageAsync(Package package)
        {
            SendPackageAsyncBase(package);
        }

        /// <summary>
        /// Sends the specified package asynchronously to the server.
        /// </summary>
        /// <returns>How many bytes were sent.</returns>
        public Task<int> SendPackageTaskAsync(Package package)
        {
            return SendPackageTaskAsyncBase(package);
        }

        /// <summary>
        /// Sends the specified package asynchronously to the server.
        /// </summary>
        /// <returns>How many bytes were sent.</returns>
        public Task<int> SendPackageTaskAsync(int command, ISerializable serializable)
        {
            return SendPackageTaskAsyncBase(command, serializable);
        }

        /// <summary>
        /// Sends the specified package asynchronously to the server.
        /// </summary>
        public void SendPackageAsync(int command, ISerializable serializable)
        {
            SendPackageAsyncBase(command, serializable);
        }

        /// <summary>
        /// Sends the package synchronously to the server.
        /// </summary>
        /// <returns>Returns the amount of data sent in bytes.</returns>
        public int SendPackage(int command, ISerializable serializable)
        {
            return SendPackageBase(command, serializable);
        }

        /// <summary>
        /// Sends the package synchronously to the server.
        /// </summary>
        /// <returns>Returns the amount of data sent in bytes.</returns>
        public int SendPackage(Package package)
        {
            return SendPackageBase(package);
        }
        #endregion
    }
}
