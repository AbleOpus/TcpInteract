using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TcpInteract.Forms
{
    /// <summary>
    /// Represents a basic login prompt.
    /// </summary>
    public partial class LoginForm : Form
    {
        private LoginFormState loginState;
        /// <summary>
        /// Gets or sets the state which reflects the clients current state.
        /// </summary>
        [Description("Determines the state which reflects the clients current state."), 
            DefaultValue(LoginFormState.Idle), Category("Appearance")]
        public LoginFormState LoginState
        {
            get { return loginState; }
            set
            {
                loginState = value;

                switch (loginState)
                {
                    case LoginFormState.LoggedIn:
                        buttonCancel.Enabled = false;
                        buttonLogin.Enabled = false;
                        labelStatus.Text = "Logged In";
                        break;

                    case LoginFormState.Connecting:
                        buttonCancel.Enabled = true;
                        buttonLogin.Enabled = false;
                        labelStatus.Text = "Connecting...";
                        break;

                    case LoginFormState.LoggingIn:
                        buttonCancel.Enabled = true;
                        buttonLogin.Enabled = false;
                        labelStatus.Text = "Logging in...";
                        break;

                    case LoginFormState.Idle:
                        buttonCancel.Enabled = false;
                        buttonLogin.Enabled = true;
                        labelStatus.Text = "Idle";
                        break;

                    case LoginFormState.Canceling:
                        buttonCancel.Enabled = false;
                        buttonLogin.Enabled = false;
                        labelStatus.Text = "Aborting...";
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the status to display in the status bar.
        /// </summary>
        [Browsable(false), Description("The status to display in the status bar."), 
            DefaultValue("Idle"), Category("Appearance")]
        public string Status
        {
            get { return labelStatus.Text; }
            set { labelStatus.Text = value; }
        }

        /// <summary>
        /// Gets or sets the inputted address.
        /// </summary>
        [Description("The inputted address."), DefaultValue(""), Category("Appearance")]
        public string Address
        {
            get { return textBoxAddress.Text; }
            set { textBoxAddress.Text = value; }
        }

        /// <summary>
        /// Gets or sets the inputted client name.
        /// </summary>
        [Description("The inputted client name."), DefaultValue(""), Category("Appearance")]
        public string ClientName
        {
            get { return textBoxUserName.Text; }
            set { textBoxUserName.Text = value; }
        }

        /// <summary>
        /// Occurs when the login button is clicked.
        /// </summary>
        [Description("Occurs when the login button is clicked."), Category("Action")]
        public event EventHandler LoginButtonClick
        {
            add { buttonLogin.Click += value; }
            remove { buttonLogin.Click -= value; }
        }

        /// <summary>
        /// Occurs when the cancel button is clicked.
        /// </summary>
        [Description("Occurs when the cancel button is clicked."), Category("Action")]
        public event EventHandler CancelButtonClick
        {
            add { buttonCancel.Click += value; }
            remove { buttonCancel.Click -= value; }
        }

        public LoginForm()
        {
            InitializeComponent();
        }
    }
}
