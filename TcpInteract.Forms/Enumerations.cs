namespace TcpInteract.Forms
{
    /// <summary>
    /// Specifies the login state of a <see cref="LoginForm"/>.
    /// </summary>
    public enum LoginFormState
    {
        /// <summary>
        /// The corresponding client is not doing anything.
        /// </summary>
        Idle,
        /// <summary>
        /// The corresponding client is connecting.
        /// </summary>
        Connecting,
        /// <summary>
        /// The corresponding client is logging in.
        /// </summary>
        LoggingIn,
        /// <summary>
        /// The corresponding client is logged in.
        /// </summary>
        LoggedIn,
        /// <summary>
        /// The corresponding client has request that the connection attempt be aborted.
        /// </summary>
        Canceling
    }
}
