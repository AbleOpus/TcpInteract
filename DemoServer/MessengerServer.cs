using System.Collections.Generic;
using System.Linq;
using TcpInteract;
using DemoShared;

namespace DemoServer
{
    public sealed class MessengerServer : ServerBase
    {
        /// <summary>
        /// The message history for this session.
        /// </summary>
        private readonly List<InstantMessageContent> messages = new List<InstantMessageContent>();

        public MessengerServer(int port) : base(port) { }

        /// <summary>
        /// Evaluates a client name to see if it is valid. If this method yields false,
        /// the corresponding client will be rejected.
        /// </summary>
        /// <param name="clientName">The client name to validate.</param>
        /// <returns>True, if the client name is longer than 3 characters.</returns>
        protected override bool ClientNameValidPredicate(string clientName)
        {
            return clientName.Length > 3;
        }

        protected override void Synchronize(ServerSideClient client)
        {
            base.Synchronize(client);

            foreach (var message in messages)
                client.SendPackageAsync((int)Commands.InstantMessage, message);
        }

        protected override void OnPackageReceived(ServerSideClient client, Package package)
        {
            switch ((Commands)package.Command)
            {
                // Broadcast messages to all clients.
                case Commands.InstantMessage:
                    var message = InstantMessageContent.Deserialize(package.Content);
                    messages.Add(message);
                    Pusher.Push(message);
                    BroadcastPackageAsync(package);
                    break;

                // Send screenshot specific client.
                case Commands.Screenshot:
                    var screenshot = ScreenshotContent.Deserialize(package.Content);
                    ServerSideClient destClient = Clients.FirstOrDefault(c => c.Name == screenshot.RecieverName);
                    destClient?.SendPackageAsync(package);
                    break;

                   // Send cursor to all clients. Does not need to be deserialized because
                   // the contents of the package do not need to be analyzed.
                case Commands.CursorPosition:
                    BroadcastPackageAsync(package);
                    break;
            }
        }
    }
}
