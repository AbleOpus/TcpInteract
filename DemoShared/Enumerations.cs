namespace DemoShared
{
    /// <summary>
    /// Represents commands for sending and receiving data packages.
    /// </summary>
    public enum Commands
    {
        /// <summary>
        /// Client To Client: Indicates the contents of the corresponding Package is an <see cref="InstantMessageContent"/>.
        /// </summary>
        InstantMessage,
        /// <summary>
        /// Client To Client: Indicates the contents of the corresponding package is a <see cref="ScreenshotContent"/>.
        /// </summary>
        Screenshot,
        /// <summary>
        /// Client To Clients: Indicates the contents of the corresponding package is <see cref="ClientCursorContent"/>.
        /// </summary>
        CursorPosition
    }
}
