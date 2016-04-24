using System;
using System.Data;
using System.Net;
using System.Threading.Tasks;

namespace TcpInteract
{
    /// <summary>
    /// Provides network-related functionality.
    /// </summary>
    public class Networking
    {
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
