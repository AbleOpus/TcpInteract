using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DemoShared
{
    /// <summary>
    /// Represents a memory bitmap compressed as portable network graphics.
    /// </summary>
    [Serializable]
    public class Png
    {
        private readonly byte[] data;

        /// <summary>
        /// Gets the size, in bytes, of this png.
        /// </summary>
        public int Size => data.Length;

        /// <summary>
        /// Initializes a new instance of the <see cref="Png"/> class
        /// with the specified argument.
        /// </summary>
        /// <param name="bitmap">The bitmap to compress to png.</param>
        public Png(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                data = stream.ToArray();
            }
        }

        /// <summary>
        /// Converts from png to bitmap.
        /// </summary>
        public static explicit operator Bitmap(Png png)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(png.data, 0, png.data.Length);
                return new Bitmap(stream);
            }
        }

        /// <summary>
        /// Converts from bitmap to png.
        /// </summary>
        public static explicit operator Png(Bitmap bitmap)
        {
            return new Png(bitmap);
        }
    }
}
