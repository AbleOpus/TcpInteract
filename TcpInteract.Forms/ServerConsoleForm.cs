using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TcpInteract.Forms
{
    public partial class ServerConsoleForm : Form
    {
        public event EventHandler<string> CommandSubmitted = delegate { };

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RichTextBox OutputTextBox => textBoxOutput;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RichTextBox InputTextBox => textBoxOutput;

        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                textBoxOutput.BackColor = textBoxInput.BackColor = value;
            }
        }

        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                textBoxOutput.ForeColor = textBoxInput.ForeColor = value;
            }
        }

        public ServerConsoleForm()
        {
            InitializeComponent();
        }

        public void ClearOutput()
        {
            textBoxOutput.Clear();
        }

        public bool WordWrapOutput
        {
            get { return textBoxOutput.WordWrap; }
            set { textBoxOutput.WordWrap = value; }
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
