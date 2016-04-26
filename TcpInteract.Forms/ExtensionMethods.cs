using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TcpInteract.Forms
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Scroll to the bottom of the text content.
        /// </summary>
        /// <param name="textBox"></param>
        public static void ScrollToBottom(this RichTextBox textBox)
        {
            if (textBox.TextLength > 0)
            {
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToCaret();
            }
        }

        /// <summary>
        /// Appends a new line of text to the control.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="text">The text to append.</param>
        public static void AppendLine(this RichTextBox textBox, string text)
        {
            textBox.AppendText(text + Environment.NewLine);
        }
    }
}
