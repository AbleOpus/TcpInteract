using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TcpInteract.Forms
{
    /// <summary>
    /// Represents Form for controlling a server via a command-line interface.
    /// </summary>
    public partial class ServerConsoleForm : Form
    {
        /// <summary>
        /// Occurs when a command has been submitted with the input control.
        /// </summary>
        public event EventHandler<string> CommandSubmitted = delegate { };

        /// <summary>
        /// Gets the RichTextBox used for output. 
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RichTextBox OutputTextBox => textBoxOutput;

        /// <summary>
        /// Gets the RichTextBox used for input. 
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RichTextBox InputTextBox => textBoxOutput;

        /// <summary>
        /// Gets or sets the foreground color of the form and its children.
        /// </summary>
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                textBoxOutput.BackColor = textBoxInput.BackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the form and its children.
        /// </summary>
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                textBoxOutput.ForeColor = textBoxInput.ForeColor = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConsoleForm"/> class.
        /// </summary>
        public ServerConsoleForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clears the output window.
        /// </summary>
        public void ClearOutput()
        {
            textBoxOutput.Clear();
        }

        /// <summary>
        /// Prints the specified lines to the output box.
        /// </summary>
        /// <param name="lines">The lines to print.</param>
        public void WriteLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
                textBoxOutput.AppendLine(line);

            textBoxOutput.ScrollToBottom();
        }

        /// <summary>
        /// Prints the specified line to the output box.
        /// </summary>
        /// <param name="line">The line to print.</param>
        public void WriteLine(string line = "")
        {
            textBoxOutput.AppendLine(line);
            textBoxOutput.ScrollToBottom();
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                CommandSubmitted(this, textBoxInput.Text);
                textBoxInput.Clear();
            }
        }
    }
}
