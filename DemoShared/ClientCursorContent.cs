using System;
using System.Drawing;
using TcpInteract;

namespace DemoShared
{
    /// <summary>
    /// Represents the position of a remote client's cursor.
    /// </summary>
    [Serializable]
    public class ClientCursorContent : SerializableContent<ClientCursorContent>
    {
        /// <summary>
        /// Gets the position of the clients cursor.
        /// </summary>
        public Point CursorPosition { get; }

        /// <summary>
        /// Gets the name of the associated client.
        /// </summary>
        public string ClientName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCursorContent"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="cursorPosition">The position of the clients cursor.</param>
        /// <param name="clientName">The name of the associated client.</param>
        public ClientCursorContent(Point cursorPosition, string clientName)
        {
            CursorPosition = cursorPosition;
            ClientName = clientName;
        }

        public override string ToString()
        {
            return $"{ClientName} - {CursorPosition}";
        }
    }
}
