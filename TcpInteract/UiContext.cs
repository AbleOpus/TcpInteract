using System;
using System.Diagnostics;
using System.Threading;

namespace TcpInteract
{
    /// <summary>
    /// Represents the UI's execution context.
    /// </summary>
    internal sealed class UiContext
    {
        /// <summary>
        /// The only instance of the <see cref="UiContext"/> class.
        /// </summary>
        public static readonly UiContext Default = new UiContext();

        private SynchronizationContext syncContext;
        /// <summary>
        /// Gets whether invoke is required to run logic on the UI thread.
        /// </summary>
        public bool InvokeRequired => SynchronizationContext.Current != syncContext;

        /// <summary>
        /// Gets whether this instance has been initialized.
        /// </summary>
        public bool Initialized => syncContext != null;

        private UiContext()
        {
        }

        /// <summary>
        /// Initializes the internal synchronization context. Only call this when the
        /// UI <see cref="SynchronizationContext"/> is available (after a UI element has been created).
        /// </summary>
        /// <returns>True, if the context was successfully initialized. Otherwise false.</returns>
        /// <exception cref="InvalidOperationException">The UI Context could not initialize at this time.</exception>
        public void Initialize(SynchronizationContext context = null)
        {
            syncContext = context ?? SynchronizationContext.Current;

            if (syncContext == null)
                throw new InvalidOperationException("The UI Context could not initialize at this time.");
        }

        /// <summary>
        /// Invokes an <see cref="Action"/> on the UI thread.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Invoke(Action action)
        {
            InvokeCommon(action);
            syncContext.Post(delegate { action.Invoke(); }, null);
        }

        /// <summary>
        /// Invokes an <see cref="Action"/> on the UI thread.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Invoke<T>(Action<T> action, T args)
        {
            InvokeCommon(action);
            syncContext.Post(delegate { action.Invoke(args); }, null);
        }

        /// <summary>
        /// Invokes an <see cref="Action"/> on the UI thread.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Invoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            InvokeCommon(action);
            syncContext.Post(delegate { action.Invoke(arg1, arg2); }, null);
        }

        private void InvokeCommon(Delegate action)
        {
            if (action == null)
                throw new ArgumentException("Value cannot be null.", nameof(action));

#if DEBUG
            if (!InvokeRequired)
            {
                Debug.WriteLine($@"Invoke not required with method ""{action.Method}""");
            }
#endif
        }
    }
}
