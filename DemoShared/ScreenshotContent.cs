using System;
using TcpInteract;

namespace DemoShared
{
    /// <summary>
    /// Represents a screenshot to be sent to a specific client.
    /// </summary>
    [Serializable]
    public class ScreenshotContent : SerializableContent<ScreenshotContent>
    {
        /// <summary>
        /// Gets the screenshot as a png.
        /// </summary>
        public Png Image { get; }

        /// <summary>
        /// Gets the name of the client that sent the screenshot.
        /// </summary>
        public string SenderName { get; }

        /// <summary>
        /// Gets the name of the client which receives this screenshot.
        /// </summary>
        public string RecieverName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenshotContent"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="image">The screenshot as a png.</param>
        /// <param name="senderName">The name of the client that sent the screenshot.</param>
        /// <param name="recieverName">The name of the client which receives this screenshot.</param>
        public ScreenshotContent(Png image, string senderName, string recieverName)
        {
            Image = image;
            SenderName = senderName;
            RecieverName = recieverName;
        }
    }
}
