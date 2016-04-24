using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DemoShared;
using TcpInteract;

namespace DemoClient
{
    /// <summary>
    /// Represents a TCP client used for instant messaging.
    /// </summary>
    public class MessengerClient : ClientSideClient
    {
        private readonly Timer timerSend = new Timer();

        private readonly BindingList<ClientCursorContent> clientCursorPositions = new BindingList<ClientCursorContent>();
        public IBindingList ClientCursorPositions => clientCursorPositions;

        public MessengerClient()
        {
            timerSend.Tick += delegate
            {
                if (Socket != null && Socket.Connected && Status == ClientStatus.LoggedIn)
                {
                    var args = new ClientCursorContent(Cursor.Position, Name);
                    SendPackageAsyncBase((int)Commands.CursorPosition, args);
                }
            };
            timerSend.Start();
        }

        /// <summary>
        /// Sends an instant message asynchronously.
        /// </summary>
        public void SendMessageAsync(string message)
        {
            SendPackageAsyncBase((int)Commands.InstantMessage, new InstantMessageContent(message, Name));
        }

        /// <summary>
        /// Sends a screenshot asynchronously.
        /// </summary>
        /// <param name="image">The image to send.</param>
        /// <param name="toClient">The client to send to.</param>
        /// <returns>How much data, in bytes, successfully sent.</returns>
        public void SendScreenAsync(Bitmap image, string toClient)
        {
            var screenshot = new ScreenshotContent((Png)image, Name, toClient);
            SendPackageTaskAsyncBase((int)Commands.Screenshot, screenshot);
        }

        protected override void OnLoggedOut(LogoutContent content)
        {
            base.OnLoggedOut(content);
            RemoveClientCursorPosition(content.ClientName);
        }

        private void RemoveClientCursorPosition(string clientName)
        {
            var find = clientCursorPositions.FirstOrDefault(c => c.ClientName == clientName);

            if (find != null)
            {
                clientCursorPositions.Remove(find);
            }
        }

        protected override void OnPackageReceived(Package package)
        {
            base.OnPackageReceived(package);

            switch ((Commands)package.Command)
            {
                case Commands.InstantMessage:
                    Pusher.Push(InstantMessageContent.Deserialize(package.Content));
                    break;

                case Commands.Screenshot:
                    Pusher.Push(ScreenshotContent.Deserialize(package.Content));
                    break;

                case Commands.CursorPosition:
                    var args = ClientCursorContent.Deserialize(package.Content);
                    var find = clientCursorPositions.FirstOrDefault(c => c.ClientName == args.ClientName);

                    if (find == null)
                    {
                        clientCursorPositions.Add(args);
                    }
                    else
                    {
                        int index = clientCursorPositions.IndexOf(find);

                        if (index != -1)
                            clientCursorPositions[index] = args;
                    }
                    break;
            }
        }

        public override void Dispose()
        {
            timerSend.Dispose();
            base.Dispose();
        }
    }
}
