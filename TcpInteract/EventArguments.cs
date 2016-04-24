using System;

namespace TcpInteract
{
    /// <summary>
    /// Provides event arguments for the <see cref="ClientSideClient.ConnectionAttemptFailed"/> event.
    /// </summary>
    public class ConnectionAttemptFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets how many connection attempts have been made.
        /// </summary>
        public int ConnectionAttempts { get; }

        /// <summary>
        /// Gets whether the maximum amount of attempts is reached.
        /// </summary>
        public bool ReachedMaximumAttempts { get; }

        /// <summary>
        /// Gets whether attempting to connect was aborted. (Not connection itself).
        /// </summary>
        public bool Aborted { get; }

        /// <summary>
        /// Gets whether the connector has stopped trying to connect. If <see cref="Aborted"/> or
        /// <see cref="ReachedMaximumAttempts"/> is true, then the value of the <see cref="EndOfAttempts"/>
        /// property will be true.
        /// </summary>
        public bool EndOfAttempts => ReachedMaximumAttempts || Aborted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionAttemptFailedEventArgs"/> class
        /// with the specified arguments.
        /// </summary>
        /// <param name="connectionAttempts">How many connection attempts have been made.</param>
        /// <param name="reachedMaximumAttempts">Whether the maximum amount of attempts is reached.</param>
        /// <param name="aborted">Whether attempting to connect was aborted. (Not connection itself).</param>
        public ConnectionAttemptFailedEventArgs(int connectionAttempts, bool reachedMaximumAttempts, bool aborted)
        {
            ConnectionAttempts = connectionAttempts;
            ReachedMaximumAttempts = reachedMaximumAttempts;
            Aborted = aborted;
        }
    }
}
