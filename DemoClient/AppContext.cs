using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using TcpInteract;
using DemoClient.Forms;
using DemoClient.Properties;
using DemoShared;

namespace DemoClient
{
    internal class AppContext : ApplicationContext
    {
        private readonly MessengerForm formMessenger = new MessengerForm();
        private readonly LoginForm formLogin = new LoginForm();
        private readonly MessengerClient client = new MessengerClient();

        private const int PORT = 2059;

        public AppContext()
        {
            LoadSettings();
            // Setup login form.
            MainForm = formLogin;
            formLogin.LoginButtonClick += FormLoginOnButtonLoginClick;
            formLogin.CancelButtonClick += delegate
            {
                client.AbortConnect();
                formLogin.LoggingIn = false;
                formLogin.Status = "Aborted";
            };

            // Setup messenger form.
            formMessenger.Closing += (s, e) =>
            {
                e.Cancel = true;
                client.Logout();
                formMessenger.Hide();
                formLogin.Show();
                formLogin.LoggingIn = false;
            };
            formMessenger.TimeOutClientButtonClick += delegate { client.Dispose(); };
            formMessenger.SendScreenClick += FormMessengerOnSendScreenClick;
            formMessenger.SendMessage += (s, e) => client.SendMessageAsync(e);
            formMessenger.ListBoxUsersDataSource = client.ClientCursorPositions;

            // Hook client events.
            client.ConnectionAttemptFailed += (s, e) =>
            {
                if (e.Aborted)
                {
                    formLogin.Status = "Aborted.";
                }
                else if (e.ReachedMaximumAttempts)
                {
                    formLogin.Status = "Reached max attempts.";
                }
                else
                {
                    formLogin.Status = "Attempt #" + e.ConnectionAttempts;
                }
            };
            client.StatusChanged += ClientStatusChanged;

            client.Pusher.Bind<ServerClosedContent>(content =>
            {
                MessageBox.Show("The server has closed.", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                formMessenger.Close();
            });
            client.Pusher.Bind<ConnectionRefusedContent>(ClientOnConnectionRefused);
            client.Pusher.Bind<LoginContent>(e => formMessenger.SubmitMessage($@"{e.ClientName}: has logged in."));
            client.Pusher.Bind<LogoutContent>(ClientOnClientLoggedOut);
            client.Pusher.Bind<InstantMessageContent>(e => formMessenger.SubmitMessage($@"{e.SenderName}: {e.Message}"));
            client.Pusher.Bind<ScreenshotContent>(screenshot => formMessenger.Screenshot = (Bitmap)screenshot.Image);
        }

        protected override void ExitThreadCore()
        {
            client.Dispose();
            formMessenger.Dispose();
            formLogin.Dispose();
            Settings.Default.Save();
            base.ExitThreadCore();
        }

        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, Application.ProductName,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static Bitmap CaptureScreen(Rectangle bounds)
        {
            Bitmap capture = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(capture))
                graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);

            return capture;
        }

        private void LoadSettings()
        {
            if (String.IsNullOrWhiteSpace(Settings.Default.ClientName))
                Settings.Default.ClientName = Environment.UserName;

            formLogin.ClientName = Settings.Default.ClientName;
            formLogin.Address = Settings.Default.Address;
        }

        private void ClientStatusChanged(object sender, EventArgs e)
        {
            switch (client.Status)
            {
                case ClientStatus.Connected:
                    formLogin.Status = "Connected. Awaiting login approval.";
                    formLogin.LoggingIn = true;
                    break;

                case ClientStatus.Disconnected:
                    formLogin.Status = "Idle.";
                    formLogin.LoggingIn = false;
                    break;

                case ClientStatus.LoggedIn:
                    formLogin.Status = "Logged in.";
                    formLogin.LoggingIn = false;
                    formLogin.Hide();
                    formMessenger.SetClientName(client.Name);
                    formMessenger.Show();
                    client.Synchronize();
                    formMessenger.SetDebugInfo(ClientInfo.FromClient(client).GetLines());
                    break;
            }
        }

        private void ClientOnClientLoggedOut(LogoutContent content)
        {
            string message;

            switch (content.Reason)
            {
                case LogoutReason.Kicked:
                    message = $@"{content.ClientName}: was kicked. Reason: {content.Message}";

                    if (content.ClientName == client.Name)
                    {
                        MessageBox.Show($"You have been kicked.\nReason: {content.Reason}.", Application.ProductName,
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        formMessenger.Close();
                    }
                    break;

                case LogoutReason.TimedOut:
                    message = $@"{content.ClientName}: timed out.";
                    break;

                case LogoutReason.UserSpecified:
                    message = $@"{content.ClientName}: logged out.";
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }

            formMessenger.SubmitMessage(message);
        }

        private void ClientOnConnectionRefused(ConnectionRefusedContent e)
        {
            formLogin.Status = "Connection refused: " + e.Reason;
            formLogin.LoggingIn = false;
        }

        private void FormLoginOnButtonLoginClick(object sender, EventArgs e)
        {
            IPAddress address;

            try
            {
                address = IPAddress.Parse(formLogin.Address);
            }
            catch (FormatException)
            {
                ShowErrorMessage("Invalid address format.");
                return;
            }

            client.Name = formLogin.ClientName;
            formLogin.LoggingIn = true;
            client.EndPoint = new IPEndPoint(address, PORT);

            try
            {
                client.RequestLogin();
            }
            catch (InvalidOperationException ex)
            {
                formLogin.Status = ex.Message;
                formLogin.LoggingIn = false;
            }
            catch (AlreadyLoggedInException ex)
            {
                formLogin.Status = ex.Message;
                formLogin.LoggingIn = false;
            }
        }

        private void FormMessengerOnSendScreenClick(object sender, string clientName)
        {
            if (clientName == null)
            {
                ShowErrorMessage("No user selected to send screen to.");
            }
            else if (clientName == client.Name)
            {
                ShowErrorMessage("To send screen, select another user, not the current one.");
            }
            else
            {
                using (Bitmap capture = CaptureScreen(Screen.GetBounds(formMessenger.Bounds)))
                    client.SendScreenAsync(capture, clientName);
            }
        }
    }
}