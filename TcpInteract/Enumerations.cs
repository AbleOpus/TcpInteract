using System;

namespace TcpInteract
{
    // Enum starts at -100 so consumer commands can be defined without any concern for numbering.

    /// <summary>
    /// Commands to be used for both the server and client.
    /// </summary>
    public enum BaseCommand
    {
        /// <summary>
        /// Client to Server: The client is requesting initial data so it may synchronize with
        /// the server and/or other clients.
        /// </summary>
        Sync = -100,
        /// <summary>
        /// Client To Server: A request to login.
        /// Server To Client: The server has approved of the login.
        /// </summary>
        Login,
        /// <summary>
        /// Server To Client: That the clients attempt to connect or login has been refused.
        /// The content of the corresponding package will contain an error code specifying what was invalid.
        /// </summary>
        ConnectionRefused,
        /// <summary>
        /// Server To Client: A client has logged out.
        ///  Client To Server: A client has disconnected.
        /// </summary>
        Logout,
        /// <summary>
        /// Server To Client: The server has been closed.
        /// </summary>
        ServerClosed,
        /// <summary>
        /// Client To Server: Requests a list of client names.
        /// Server To Client: The content is a list of client names separated by '|'.
        /// </summary>
        ClientNames
    }

    /// <summary>
    /// Describes the possible reasons why a connection may be refused.
    /// </summary>
    [Flags]
    public enum ConnectionRefusedReason
    {
        /// <summary>
        /// No reason.
        /// </summary>
        None,
        /// <summary>
        /// Name is null, empty, or whitespace.
        /// </summary>
        EmptyName,
        /// <summary>
        /// A logged in client already has that name.
        /// </summary>
        NameExists,
        /// <summary>
        /// The client name has been invalidated by a custom implementation.
        /// </summary>
        CustomInvalid,
        /// <summary>
        /// The client connected but did not log in soon enough.
        /// </summary>
        NoLogin
    }

    /// <summary>
    /// Specifies the reason for the logout.
    /// </summary>
    public enum LogoutReason
    {
        /// <summary>
        /// The user has initiated the logout. This almost always indicates a graceful disconnect.
        /// </summary>
        UserSpecified,
        /// <summary>
        /// The client has lost connection ungracefully.
        /// </summary>
        TimedOut,
        /// <summary>
        /// The client has been kicked by the server.
        /// </summary>
        Kicked
    }

    /// <summary>
    /// Specifies the connection status of the client.
    /// </summary>
    public enum ClientStatus
    {
        /// <summary>
        /// The client is not connected to any server.
        /// </summary>
        Disconnected,
        /// <summary>
        /// The client is connecting to a server but not logged in.
        /// </summary>
        Connecting,
        /// <summary>
        /// The client is connected to a server but not logged in.
        /// </summary>
        Connected,
        /// <summary>
        /// The client is logged in. It has been approved by the server.
        /// </summary>
        LoggedIn
    }
}
