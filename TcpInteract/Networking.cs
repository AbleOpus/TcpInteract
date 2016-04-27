using System;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace TcpInteract
{
    /// <summary>
    /// Provides network-related functionality.
    /// </summary>
    public static class Networking
    {
        /// <summary>
        /// Serializes the specified object using <see cref="BinaryFormatter"/>.
        /// </summary>
        public static byte[] Serialize<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserialize data using <see cref="BinaryFormatter"/>.
        /// </summary>
        public static T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return (T)(new BinaryFormatter().Deserialize(stream));
            }
        }

        /// <summary>
        /// Gets the remote IP address of this machine.
        /// </summary>
        /// <exception cref="DataException">The web content downloaded did not contain an address.</exception>
        /// <exception cref="WebException"></exception>
        public static string GetPublicIpString()
        {
            using (WebClient client = new WebClient())
            {
                var uri = new Uri(@"https://api.ipify.org/");
                return client.DownloadString(uri);
            }
        }

        /// <summary>
        /// Gets the remote IP address of this machine asynchronously.
        /// </summary>
        /// <exception cref="DataException">The web content downloaded did not contain an address.</exception>
        /// <exception cref="WebException"></exception>
        public static Task<string> GetPublicIpStringTaskAsync()
        {
            return Task.Run(() => GetPublicIpString());
        }
    }
}
