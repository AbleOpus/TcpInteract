using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TcpInteract
{
    /// <summary>
    /// Provides extension methods to ease the development of networked applications.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets indistinct elements from a sequence by using the default equality comparer to compare values.
        /// </summary>
        /// <param name="items"></param>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <returns>A <see cref="IEnumerable&lt;T&gt;"/> that contains distinct elements from the source sequence.</returns>
        public static IEnumerable<T> Indistinct<T>(this IEnumerable<T> items)
        {
            return items.Where(item => items.Count(i => i.Equals(item)) >= 2).Distinct();
        }

        /// <summary>
        /// Combines two byte arrays into one larger array.
        /// </summary>
        internal static byte[] Append(this byte[] b1, byte[] b2)
        {
            int length = b1.Length + b2.Length;
            byte[] result = new byte[length];
            Buffer.BlockCopy(b1, 0, result, 0, b1.Length);
            Buffer.BlockCopy(b2, 0, result, b1.Length, b2.Length);
            return result;
        }

        /// <summary>
        /// Trims the specified amount of bytes off of the start of the array.
        /// </summary>
        internal static byte[] TrimStart(this byte[] data, int amount)
        {
            int length = data.Length - amount;
            byte[] result = new byte[length];
            Buffer.BlockCopy(data, amount, result, 0, length);
            return result;
        }
    }
}
