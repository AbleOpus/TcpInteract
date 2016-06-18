using System;
using System.Net;
using System.Text;

namespace TcpInteract
{
    /// <summary>
    /// Represents significant information about a <see cref="ClientBase"/> instance.
    /// </summary>
    public sealed class ClientInfo
    {
        /// <summary>
        /// Gets the connection status of the client.
        /// </summary>
        public ClientStatus Status { get; }

        /// <summary>
        /// Gets the name of the client.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the time in which this client has established a connection. 
        /// </summary>
        public DateTime? ConnectionTime { get; }

        /// <summary>
        /// Gets the time in which this client has logged in. 
        /// </summary>
        public DateTime? LoggedInTime { get; }

        /// <summary>
        /// Gets the IP address of the client.
        /// </summary>
        public IPAddress IP { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientInfo"/> class with the
        /// specified arguments.
        /// </summary>
        /// <param name="status">The connection status of the client.</param>
        /// <param name="name">The name of the client.</param>
        /// <param name="connectionTime">The time in which this client has established a connection. </param>
        /// <param name="loggedInTime">The time in which this client has logged in. </param>
        /// <param name="IP">The IP address of the client.</param>
        private ClientInfo(ClientStatus status, string name, DateTime? connectionTime, DateTime? loggedInTime, IPAddress IP)
        {
            Status = status;
            Name = name;
            ConnectionTime = connectionTime;
            LoggedInTime = loggedInTime;
            this.IP = IP;
        }

        /// <summary>
        /// Gets the significant information from a client.
        /// </summary>
        /// <param name="client">The client to extract the information from.</param>
        /// <returns>Null, if <para>client</para> is null.</returns>
        public static ClientInfo FromClient(ClientBase client)
        {
            if (client == null)
                return null;

            return new ClientInfo(
                client.Status,
                client.Name,
                client.ConnectionTime,
                client.LoggedInTime,
                ((IPEndPoint)client.Socket.RemoteEndPoint).Address);
        }

        /// <summary>
        /// Gets the client info as an array of strings.
        /// </summary>
        /// <param name="format">The format to use for each line. {0} is the name
        /// of the property and {1} is the value.</param>
        public string[] GetLines(string format = @"{0}: {1}")
        {
            string ctVal = ConnectionTime?.ToString("hh:mm:ss tt") ?? "N/A";
            string litVal = LoggedInTime?.ToString("hh:mm:ss tt") ?? "N/A";

            return new[]
            {
                String.Format(format, nameof(Name), Name),
                String.Format(format, nameof(Status), Status),
                String.Format(format, nameof(ConnectionTime), ctVal),
                String.Format(format, nameof(LoggedInTime), litVal),
                String.Format(format, nameof(IP), IP)
            };
        }

        /// <summary>
        /// Converts the instance to a single-line string.
        /// </summary>
        public override string ToString()
        {
            return $"Status: {Status}, Name: {Name}, ConnectionTime: {ConnectionTime}, LoggedInTime: {LoggedInTime}, IP: {IP}";
        }

        /// <summary>
        /// Converts to a string that represents the current object.
        /// </summary>
        /// <param name="format">The format to use for individual lines. {0} is the name
        /// of the property and {1} is the value.</param>
        /// <param name="lineSeparator">The string to use in between lines.</param>
        public string ToString(string format, string lineSeparator = "\r\n")
        {
            var lines = GetLines(format);
            StringBuilder SB = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == lines.Length - 1)
                {
                    SB.AppendLine(lines[i]);
                }
                else
                {
                    SB.AppendLine(lines[i] + lineSeparator);
                }
            }

            return SB.ToString();
        }
    }
}
