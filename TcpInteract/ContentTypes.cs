using System;

namespace TcpInteract
{
    /// <summary>
    /// Content that indicates the server has closed.
    /// </summary>
    [Serializable]
    public class ServerClosedContent : SerializableContent<ServerClosedContent>
    {
        /// <summary>
        /// Gets the message the server has for its clients. Example: The reason why
        /// the server was closed.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerClosedContent"/> class
        /// with the specified argument.
        /// </summary>
        /// <param name="message">The message the server has for its clients.</param>
        public ServerClosedContent(string message)
        {
            Message = message;
        }
    }

    /// <summary>
    /// Content that indicates what client was Kicked and for what reason.
    /// </summary>
    [Serializable]
    public class ConnectionRefusedContent : SerializableContent<ConnectionRefusedContent>
    {
        /// <summary>
        /// Gets the reason in which the indicated client has been refused.
        /// </summary>
        public ConnectionRefusedReason Reason { get; }

        /// <summary>
        /// Gets the name of the client that has been refused.
        /// </summary>
        public string ClientName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionRefusedContent"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="reason">The reason in which the indicated client has been refused.</param>
        /// <param name="clientName">The name of the client that has been refused.</param>
        public ConnectionRefusedContent(ConnectionRefusedReason reason, string clientName)
        {
            Reason = reason;
            ClientName = clientName;
        }
    }

    /// <summary>
    /// Content that lists the names of all clients logged in.
    /// </summary>
    [Serializable]
    public class ClientNamesContent : SerializableContent<ClientNamesContent>
    {
        /// <summary>
        /// Gets the client names received.
        /// </summary>
        public string[] Names { get; }

        /// <summary>
        /// Gets how many client names were received.
        /// </summary>
        public int Count => Names.Length;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientNamesContent"/> class
        /// with the specified argument.
        /// </summary>
        /// <param name="names">The client names received.</param>
        public ClientNamesContent(string[] names)
        {
            Names = names;
        }
    }

    /// <summary>
    /// Content that describes what client has logged out and for what reason.
    /// </summary>
    [Serializable]
    public class LogoutContent : SerializableContent<LogoutContent>
    {
        /// <summary>
        /// Gets the reason in which the client has logged out.
        /// </summary>
        public LogoutReason Reason { get; }

        /// <summary>
        /// Gets the name of the client that was logged out.
        /// </summary>
        public string ClientName { get; }

        /// <summary>
        /// Gets the outro message for the logout. For instance, a goodbye message
        /// for willful logouts or a server kick message describing the nature of the kick.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutContent"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="clientName">The name of the client that has logged out.</param>
        /// <param name="reason">The reason in which the client has logged out.</param>
        /// <param name="message">The outro message for the logout.</param>
        public LogoutContent(string clientName, LogoutReason reason, string message = "")
        {
            ClientName = clientName;
            Reason = reason;
            Message = message;
        }
    }

    /// <summary>
    /// Content to describe a successful client login.
    /// </summary>
    [Serializable]
    public class LoginContent : SerializableContent<LoginContent>
    {
        /// <summary>
        /// Gets the name of the client that has logged in.
        /// </summary>
        public string ClientName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginContent"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="clientName">The name of the client that has logged in.</param>
        public LoginContent(string clientName)
        {
            ClientName = clientName;
        }
    }
}
