using System;
using System.Windows.Forms;

namespace DemoClient.Forms
{
    public partial class LoginForm : Form
    {
        /// <summary>
        /// Occurs when the login button is clicked.
        /// </summary>
        public event EventHandler LoginButtonClick
        {
            add { buttonLogin.Click += value; }
            remove { buttonLogin.Click -= value; }
        }

        /// <summary>
        /// Occurs when the cancel button is clicked.
        /// </summary>
        public event EventHandler CancelButtonClick
        {
            add { buttonCancel.Click += value; }
            remove { buttonCancel.Click -= value; }
        }

        /// <summary>
        /// Gets or sets the status to display in the status bar.
        /// </summary>
        public string Status
        {
            get { return labelStatus.Text; }
            set { labelStatus.Text = value; }
        }

        /// <summary>
        /// Gets or sets the inputted address.
        /// </summary>
        public string Address
        {
            get { return textBoxAddress.Text; } 
            set { textBoxAddress.Text = value; }
        }

        /// <summary>
        /// Gets or sets the inputted client name.
        /// </summary>
        public string ClientName
        {
            get { return textBoxUserName.Text; }
            set { textBoxUserName.Text = value; }
        }

        private bool loggingIn;
        /// <summary>
        /// Gets or sets whether the client is logging in or not.
        /// </summary>
        public bool LoggingIn
        {
            get { return loggingIn; }
            set
            {
                loggingIn = value;

                if (value)
                {
                    buttonCancel.Enabled = true;
                    buttonLogin.Enabled = false;
                }
                else
                {
                    buttonLogin.Enabled = true;
                    buttonCancel.Enabled = false;
                }
            }
        }

        public LoginForm()
        {
            InitializeComponent();
        }
    }
}
