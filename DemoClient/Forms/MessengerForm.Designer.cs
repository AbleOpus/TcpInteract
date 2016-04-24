namespace DemoClient.Forms
{
    partial class MessengerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Panel panel1;
            this.richTextBoxChat = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxDebugInfo = new System.Windows.Forms.ListBox();
            this.buttonTimeOut = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSendScreen = new System.Windows.Forms.Button();
            this.listBoxClientNames = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageMessages = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.textBoxSend = new System.Windows.Forms.TextBox();
            this.tabPageScreenshot = new System.Windows.Forms.TabPage();
            this.picBoxScreeny = new DemoClient.Forms.AblePictureBox();
            panel1 = new System.Windows.Forms.Panel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageMessages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabPageScreenshot.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.SystemColors.Window;
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(this.richTextBoxChat);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new System.Windows.Forms.Padding(12);
            panel1.Size = new System.Drawing.Size(570, 290);
            panel1.TabIndex = 1;
            // 
            // richTextBoxChat
            // 
            this.richTextBoxChat.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxChat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxChat.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.richTextBoxChat.Location = new System.Drawing.Point(12, 12);
            this.richTextBoxChat.Name = "richTextBoxChat";
            this.richTextBoxChat.ReadOnly = true;
            this.richTextBoxChat.Size = new System.Drawing.Size(544, 264);
            this.richTextBoxChat.TabIndex = 1;
            this.richTextBoxChat.Text = "";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxDebugInfo);
            this.splitContainer1.Panel1.Controls.Add(this.buttonTimeOut);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.buttonSendScreen);
            this.splitContainer1.Panel1.Controls.Add(this.listBoxClientNames);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl);
            this.splitContainer1.Size = new System.Drawing.Size(833, 432);
            this.splitContainer1.SplitterDistance = 245;
            this.splitContainer1.TabIndex = 1;
            // 
            // listBoxDebugInfo
            // 
            this.listBoxDebugInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxDebugInfo.FormattingEnabled = true;
            this.listBoxDebugInfo.IntegralHeight = false;
            this.listBoxDebugInfo.Location = new System.Drawing.Point(0, 251);
            this.listBoxDebugInfo.Name = "listBoxDebugInfo";
            this.listBoxDebugInfo.Size = new System.Drawing.Size(245, 181);
            this.listBoxDebugInfo.TabIndex = 5;
            // 
            // buttonTimeOut
            // 
            this.buttonTimeOut.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonTimeOut.Location = new System.Drawing.Point(0, 228);
            this.buttonTimeOut.Name = "buttonTimeOut";
            this.buttonTimeOut.Size = new System.Drawing.Size(245, 23);
            this.buttonTimeOut.TabIndex = 6;
            this.buttonTimeOut.Text = "Time Out Client";
            this.buttonTimeOut.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 215);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "My Client:";
            // 
            // buttonSendScreen
            // 
            this.buttonSendScreen.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonSendScreen.Location = new System.Drawing.Point(0, 192);
            this.buttonSendScreen.Name = "buttonSendScreen";
            this.buttonSendScreen.Size = new System.Drawing.Size(245, 23);
            this.buttonSendScreen.TabIndex = 3;
            this.buttonSendScreen.Text = "Send Screen To Selected";
            this.buttonSendScreen.UseVisualStyleBackColor = true;
            this.buttonSendScreen.Click += new System.EventHandler(this.buttonSendScreen_Click);
            // 
            // listBoxClientNames
            // 
            this.listBoxClientNames.Dock = System.Windows.Forms.DockStyle.Top;
            this.listBoxClientNames.FormattingEnabled = true;
            this.listBoxClientNames.IntegralHeight = false;
            this.listBoxClientNames.Location = new System.Drawing.Point(0, 13);
            this.listBoxClientNames.Name = "listBoxClientNames";
            this.listBoxClientNames.Size = new System.Drawing.Size(245, 179);
            this.listBoxClientNames.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Clients:";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageMessages);
            this.tabControl.Controls.Add(this.tabPageScreenshot);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(584, 432);
            this.tabControl.TabIndex = 2;
            // 
            // tabPageMessages
            // 
            this.tabPageMessages.Controls.Add(this.splitContainer2);
            this.tabPageMessages.Location = new System.Drawing.Point(4, 22);
            this.tabPageMessages.Name = "tabPageMessages";
            this.tabPageMessages.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMessages.Size = new System.Drawing.Size(576, 406);
            this.tabPageMessages.TabIndex = 0;
            this.tabPageMessages.Text = "Messages";
            this.tabPageMessages.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(panel1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBoxSend);
            this.splitContainer2.Size = new System.Drawing.Size(570, 400);
            this.splitContainer2.SplitterDistance = 290;
            this.splitContainer2.TabIndex = 2;
            // 
            // textBoxSend
            // 
            this.textBoxSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSend.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.textBoxSend.Location = new System.Drawing.Point(0, 0);
            this.textBoxSend.Multiline = true;
            this.textBoxSend.Name = "textBoxSend";
            this.textBoxSend.Size = new System.Drawing.Size(570, 106);
            this.textBoxSend.TabIndex = 0;
            this.textBoxSend.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSend_KeyDown);
            // 
            // tabPageScreenshot
            // 
            this.tabPageScreenshot.Controls.Add(this.picBoxScreeny);
            this.tabPageScreenshot.Location = new System.Drawing.Point(4, 22);
            this.tabPageScreenshot.Name = "tabPageScreenshot";
            this.tabPageScreenshot.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageScreenshot.Size = new System.Drawing.Size(576, 406);
            this.tabPageScreenshot.TabIndex = 1;
            this.tabPageScreenshot.Text = "Screenshot";
            this.tabPageScreenshot.UseVisualStyleBackColor = true;
            // 
            // picBoxScreeny
            // 
            this.picBoxScreeny.BackColor = System.Drawing.Color.Black;
            this.picBoxScreeny.Cursor = System.Windows.Forms.Cursors.Default;
            this.picBoxScreeny.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBoxScreeny.Location = new System.Drawing.Point(3, 3);
            this.picBoxScreeny.Name = "picBoxScreeny";
            this.picBoxScreeny.Size = new System.Drawing.Size(570, 400);
            this.picBoxScreeny.SizeMode = DemoClient.Forms.DisplayMode.Zoomable;
            this.picBoxScreeny.TabIndex = 0;
            this.picBoxScreeny.Text = "ablePictureBox1";
            this.picBoxScreeny.ZoomSpeedMultiplier = 5;
            // 
            // MessengerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(857, 456);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MessengerForm";
            this.Text = "Test Client Messenger";
            panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageMessages.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabPageScreenshot.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBoxClientNames;
        private System.Windows.Forms.Button buttonSendScreen;
        private System.Windows.Forms.ListBox listBoxDebugInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageMessages;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox richTextBoxChat;
        private System.Windows.Forms.TextBox textBoxSend;
        private System.Windows.Forms.TabPage tabPageScreenshot;
        private AblePictureBox picBoxScreeny;
        private System.Windows.Forms.Button buttonTimeOut;
    }
}