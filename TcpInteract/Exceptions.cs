using System;
using TcpInteract.DebugTools;

namespace TcpInteract
{
    /// <summary>
    /// The exception that is thrown when trying to log in the client when it is already logged in.
    /// </summary>
    [Serializable]
    public class AlreadyLoggedInException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyLoggedInException"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public AlreadyLoggedInException(string message, Exception inner) : base(message, inner) { }
    }
}
