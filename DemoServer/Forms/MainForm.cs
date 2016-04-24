using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using DemoServer.Properties;
using TcpInteract;
using DemoShared;

namespace DemoServer.Forms
{
    public partial class MainForm : Form
    {
        private readonly MessengerServer server;
        private bool failedStart;

        public MainForm()
        {
            InitializeComponent();

            server = new MessengerServer(2059);
            listUsers.DataSource = server.ClientNames;

            server.Pusher.Bind<InstantMessageContent>(e => SubmitLog(e.ToString()));
            server.Pusher.Bind<LoginContent>(e => SubmitLog($@"{e.ClientName} has logged in."));
            server.Pusher.Bind<LogoutContent>(e => SubmitLog($@"{e.ClientName} has logged out. Reason: {e.Reason}."));
            server.Pusher.Bind<ConnectionRefusedContent>(SubmitConnectionRefusedContent);
        }

        private void SubmitConnectionRefusedContent(ConnectionRefusedContent content)
        {
            SubmitLog($@"{content.ClientName} has been refused, reasons: {content.Reason}.");
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            buttonEnabled.Checked = true; // Start the server.
            textBoxIP.Text = await Networking.GetPublicIpStringTaskAsync();
        }

        private void SubmitLog(string text)
        {
            richTextBoxLog.AppendText("* " + text + Environment.NewLine);

            if (richTextBoxLog.TextLength > 0)
            {
                richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
                richTextBoxLog.ScrollToCaret();
            }
        }

        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            richTextBoxLog.Clear();
        }

        private void buttonClient_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Settings.Default.OpenClientPath))
            {
                using (var dialogOpenFile = new OpenFileDialog())
                {
                    dialogOpenFile.Title = "Set the path to the demo client";
                    dialogOpenFile.InitialDirectory = Application.StartupPath;

                    if (dialogOpenFile.ShowDialog() == DialogResult.OK)
                    {
                        Settings.Default.OpenClientPath = dialogOpenFile.FileName;
                        Settings.Default.Save();
                    }
                }
            }

            if (File.Exists(Settings.Default.OpenClientPath))
            {
                // Change this to your debug client path
                Process.Start(Settings.Default.OpenClientPath);
            }
            else
            {
                MessageBox.Show("File does not exist.", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (buttonEnabled.Checked)
            {
                try
                {
                    server.Start();
                    SubmitLog("Server started.");
                }
                catch (SocketException ex)
                {
                    SubmitLog(ex.Message);
                    // Setting button enabled to false below will raise this handler again,
                    // failedStart will the else condition of this if from firing unnecessarily.
                    failedStart = true;
                    buttonEnabled.Checked = false;
                    failedStart = false;
                }
            }
            else if (!failedStart)
            {
                server.Stop();
                SubmitLog("Server stopped.");
            }
        }

        private void buttonKickSelected_Click(object sender, EventArgs e)
        {
            if (listUsers.SelectedIndex == -1)
            {
                SubmitLog("Error: No user selected.");
            }
            else
            {
                string clientName = listUsers.SelectedItem.ToString();

                try
                {
                    server.KickClient(clientName, "Because I wanted to.");
                    SubmitLog($@"""{clientName}"" kicked.");
                }
                catch (ArgumentException ex)
                {
                    SubmitLog("Error: " + ex.Message);
                }
            }
        }

        private void listUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listUsers.SelectedItem != null)
            {
                string clientName = listUsers.SelectedItem.ToString();
                var info = server.GetClientInfo(clientName);

                if (info != null)
                {
                    listBoxDebugInfo.Items.Clear();
                    listBoxDebugInfo.Items.Add("Status: " + info.Status);

                    if (info.ConnectionTime != null)
                    listBoxDebugInfo.Items.Add("Connect Time: " + info.ConnectionTime.Value.ToString("hh:mm:ss tt"));

                    if (info.LoggedInTime != null)
                        listBoxDebugInfo.Items.Add("Login Time: " + info.LoggedInTime.Value.ToString("hh:mm:ss tt"));
                }
            }
        }
    }
}
