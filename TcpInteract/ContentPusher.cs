using System;
using System.Collections.Generic;
using System.Linq;

namespace TcpInteract
{
    /// <summary>
    /// Binds <see cref="Action"/>s to <see cref="Type"/>s to provide a simple notification system.
    /// Objects are pushed to be relayed to any Type-Action binding that has the type of the object.
    /// Instead of subscribing to solid events, types are subscribed to, dynamically. All subscribed
    /// methods require a single parameter.
    /// </summary>
    public class ContentPusher
    {
        private readonly List<KeyValuePair<Type, Delegate>> bindings = new List<KeyValuePair<Type, Delegate>>();

        /// <summary>
        /// Push an argument into the <see cref="ContentPusher"/> to be handled by any Action-Type binding.
        /// </summary>
        /// <param name="arg">The argument to be passed into Actions which handle the arguments type.</param>
        /// <returns>How many actions were invoked.</returns>
        public void Push<T>(T arg)
        {
            var paramType = typeof(T);
            var arguments = new object[] { arg };

            foreach (var binder in bindings.Where(b => b.Key == paramType))
            {
                binder.Value.Method.Invoke(binder.Value.Target, arguments);
            }
        }

        /// <summary>
        /// Removes all subscriptions this delegate may have.
        /// </summary>
        /// <param name="action">The action to unsubscribe.</param>
        /// <returns>True, if 1 or more bindings were removed, otherwise false.</returns>
        public bool Unbind<T>(Action<T> action)
        {
           return bindings.RemoveAll(b => b.Value.Equals(action)) > 0;
           // return bindings.Remove(new TypeActionBinding(action, typeof(T)));
        }

        /// <summary>
        /// Subscribes an <see cref="Action"/> with one parameter.
        /// </summary>
        /// <typeparam name="T">The type of arguments the specified <see cref="Action"/> will handle.</typeparam>
        /// <param name="action">The <see cref="Action"/> to handle arguments of the specified <see cref="Type"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void Bind<T>(Action<T> action)
        {
            var binding = new KeyValuePair<Type, Delegate>(typeof(T), action);

            if (bindings.Any(b => b.Value.Equals(action)))
                throw new ArgumentException("The Action specified is already bound.");

            bindings.Add(binding);
        }
    }
}
