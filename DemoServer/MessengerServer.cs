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
