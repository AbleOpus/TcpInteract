using System;
using TcpInteract;

namespace DemoShared
{
    /// <summary>
    /// Represents a message of an instant messaging session.
    /// </summary>
    [Serializable]
    public class InstantMessageContent : SerializableContent<InstantMessageContent>
    {
        /// <summary>
        /// Gets the contents of the <see cref="InstantMessageContent"/>.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the name of the client which has sent the <see cref="InstantMessageContent"/>.
        /// </summary>
        public string SenderName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstantMessageContent"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="message">The contents of the <see cref="InstantMessageContent"/>.</param>
        /// <param name="senderName">The name of the client which has sent the <see cref="InstantMessageContent"/>.</param>
        public InstantMessageContent(string message, string senderName)
        {
            Message = message;
            SenderName = senderName;
        }

        public override string ToString()
        {
            return $"{SenderName}: {Message}";
        }
    }
}
