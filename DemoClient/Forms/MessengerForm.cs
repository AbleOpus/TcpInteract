using System;
using System.Drawing;
using System.Windows.Forms;
using DemoShared;

namespace DemoClient.Forms
{
    public partial class MessengerForm : Form
    {
        /// <summary>
        /// Occurs when the user submits a message in the send message <see cref="TextBox"/>.
        /// </summary>
        public event EventHandler<string> SendMessage = delegate { };

        /// <summary>
        /// Occurs when the send screen button has been clicked and a user is selected.
        /// </summary>
        public event EventHandler<string> SendScreenClick = delegate { };

        /// <summary>
        /// Occurs when the time out client button has been clicked.
        /// </summary>
        public event EventHandler TimeOutClientButtonClick
        {
            add { buttonTimeOut.Click += value; }
            remove { buttonTimeOut.Click -= value; }
        }

        /// <summary>
        /// Gets or sets the data source for the client names <see cref="ListBox"/>.
        /// </summary>
        public object ListBoxUsersDataSource
        {
            get {return listBoxClientNames.DataSource; }
            set { listBoxClientNames.DataSource = value; }
        }

        public Image Screenshot
        {
            get { return picBoxScreeny.Image; }
            set { picBoxScreeny.Image = value; }
        }
        /// <summary>
        /// Gets or sets whether the send <see cref="Button"/> is enabled.
        /// </summary>
        public bool SendButtonEnabled
        {
            get { return buttonSendScreen.Enabled; }
            set { buttonSendScreen.Enabled = value; }
        }

        public MessengerForm()
        {
            InitializeComponent();
        }

        public void SetDebugInfo(string[] lines)
        {
            listBoxDebugInfo.Items.Clear();
            listBoxDebugInfo.Items.AddRange(lines);
        }

        /// <summary>
        /// Updates this form to reflect the local client.
        /// </summary>
        /// <param name="name">The name of the local client.</param>
        public void SetClientName(string name)
        {
            Text = Application.ProductName + " - " + name;
        }

        /// <summary>
        /// Submits a message to the chat box.
        /// </summary>
        public void SubmitMessage(string message)
        {
            if (richTextBoxChat.TextLength > 0)
            {
                richTextBoxChat.AppendText(Environment.NewLine + Environment.NewLine);
            }

            richTextBoxChat.AppendText(message);
            richTextBoxChat.SelectionStart = richTextBoxChat.Text.Length;
            richTextBoxChat.ScrollToCaret();
        }

        private void textBoxSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift && !String.IsNullOrWhiteSpace(textBoxSend.Text))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                SendMessage(this, textBoxSend.Text);
                textBoxSend.Clear();
            }
        }

        private void buttonSendScreen_Click(object sender, EventArgs e)
        {
            if (listBoxClientNames.SelectedIndex != -1)
            {
                SendScreenClick(this, ((ClientCursorContent) listBoxClientNames.SelectedItem).ClientName);
            }
        }
    }
}
