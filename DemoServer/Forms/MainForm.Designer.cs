namespace DemoServer.Forms
{
    partial class MainForm
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
            if (disposing)
            {
                server.Dispose();
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.buttonEnabled = new System.Windows.Forms.ToolStripButton();
            this.buttonOpenClient = new System.Windows.Forms.ToolStripButton();
            this.textBoxIP = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.tableLayoutUsers = new System.Windows.Forms.TableLayoutPanel();
            this.listBoxDebugInfo = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonKickSelected = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listUsers = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonClearLog = new System.Windows.Forms.Button();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            this.tableLayoutUsers.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonEnabled,
            this.buttonOpenClient,
            this.textBoxIP,
            this.toolStripLabel1});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
            this.toolStrip.Size = new System.Drawing.Size(636, 26);
            this.toolStrip.TabIndex = 11;
            this.toolStrip.Text = "toolStrip";
            // 
            // buttonEnabled
            // 
            this.buttonEnabled.CheckOnClick = true;
            this.buttonEnabled.Image = ((System.Drawing.Image)(resources.GetObject("buttonEnabled.Image")));
            this.buttonEnabled.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonEnabled.Name = "buttonEnabled";
            this.buttonEnabled.Size = new System.Drawing.Size(69, 20);
            this.buttonEnabled.Text = "Enabled";
            this.buttonEnabled.CheckedChanged += new System.EventHandler(this.buttonEnabled_CheckedChanged);
            // 
            // buttonOpenClient
            // 
            this.buttonOpenClient.Image = ((System.Drawing.Image)(resources.GetObject("buttonOpenClient.Image")));
            this.buttonOpenClient.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOpenClient.Name = "buttonOpenClient";
            this.buttonOpenClient.Size = new System.Drawing.Size(90, 20);
            this.buttonOpenClient.Text = "Open Client";
            this.buttonOpenClient.Click += new System.EventHandler(this.buttonClient_Click);
            // 
            // textBoxIP
            // 
            this.textBoxIP.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.ReadOnly = true;
            this.textBoxIP.Size = new System.Drawing.Size(135, 23);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(20, 20);
            this.toolStripLabel1.Text = "IP:";
            // 
            // splitContainerVert
            // 
            this.splitContainerVert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerVert.Location = new System.Drawing.Point(0, 26);
            this.splitContainerVert.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainerVert.Name = "splitContainerVert";
            // 
            // splitContainerVert.Panel1
            // 
            this.splitContainerVert.Panel1.Controls.Add(this.tableLayoutUsers);
            // 
            // splitContainerVert.Panel2
            // 
            this.splitContainerVert.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.splitContainerVert.Size = new System.Drawing.Size(636, 339);
            this.splitContainerVert.SplitterDistance = 150;
            this.splitContainerVert.SplitterWidth = 3;
            this.splitContainerVert.TabIndex = 15;
            // 
            // tableLayoutUsers
            // 
            this.tableLayoutUsers.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutUsers.ColumnCount = 2;
            this.tableLayoutUsers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutUsers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutUsers.Controls.Add(this.listBoxDebugInfo, 0, 3);
            this.tableLayoutUsers.Controls.Add(this.label3, 0, 2);
            this.tableLayoutUsers.Controls.Add(this.buttonKickSelected, 1, 0);
            this.tableLayoutUsers.Controls.Add(this.label1, 0, 0);
            this.tableLayoutUsers.Controls.Add(this.listUsers, 0, 1);
            this.tableLayoutUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutUsers.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutUsers.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutUsers.Name = "tableLayoutUsers";
            this.tableLayoutUsers.RowCount = 4;
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutUsers.Size = new System.Drawing.Size(150, 339);
            this.tableLayoutUsers.TabIndex = 14;
            // 
            // listBoxDebugInfo
            // 
            this.listBoxDebugInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutUsers.SetColumnSpan(this.listBoxDebugInfo, 2);
            this.listBoxDebugInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxDebugInfo.FormattingEnabled = true;
            this.listBoxDebugInfo.IntegralHeight = false;
            this.listBoxDebugInfo.Location = new System.Drawing.Point(1, 195);
            this.listBoxDebugInfo.Margin = new System.Windows.Forms.Padding(0);
            this.listBoxDebugInfo.Name = "listBoxDebugInfo";
            this.listBoxDebugInfo.Size = new System.Drawing.Size(148, 143);
            this.listBoxDebugInfo.TabIndex = 23;
            // 
            // label3
            // 
            this.tableLayoutUsers.SetColumnSpan(this.label3, 2);
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 174);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.label3.Size = new System.Drawing.Size(144, 20);
            this.label3.TabIndex = 22;
            this.label3.Text = "Debug Info:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonKickSelected
            // 
            this.buttonKickSelected.Location = new System.Drawing.Point(71, 4);
            this.buttonKickSelected.Name = "buttonKickSelected";
            this.buttonKickSelected.Size = new System.Drawing.Size(75, 23);
            this.buttonKickSelected.TabIndex = 21;
            this.buttonKickSelected.Text = "Kick";
            this.buttonKickSelected.UseVisualStyleBackColor = true;
            this.buttonKickSelected.Click += new System.EventHandler(this.buttonKickSelected_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 1);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(62, 29);
            this.label1.TabIndex = 17;
            this.label1.Text = "Users:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listUsers
            // 
            this.listUsers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutUsers.SetColumnSpan(this.listUsers, 2);
            this.listUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listUsers.FormattingEnabled = true;
            this.listUsers.IntegralHeight = false;
            this.listUsers.Location = new System.Drawing.Point(1, 31);
            this.listUsers.Margin = new System.Windows.Forms.Padding(0);
            this.listUsers.Name = "listUsers";
            this.listUsers.Size = new System.Drawing.Size(148, 142);
            this.listUsers.TabIndex = 19;
            this.listUsers.SelectedIndexChanged += new System.EventHandler(this.listUsers_SelectedIndexChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.richTextBoxLog, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonClearLog, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(483, 339);
            this.tableLayoutPanel2.TabIndex = 14;
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel2.SetColumnSpan(this.richTextBoxLog, 2);
            this.richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLog.Location = new System.Drawing.Point(1, 31);
            this.richTextBoxLog.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.ReadOnly = true;
            this.richTextBoxLog.Size = new System.Drawing.Size(481, 307);
            this.richTextBoxLog.TabIndex = 16;
            this.richTextBoxLog.Text = "";
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 1);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(395, 29);
            this.label2.TabIndex = 14;
            this.label2.Text = "Log:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonClearLog
            // 
            this.buttonClearLog.Location = new System.Drawing.Point(404, 4);
            this.buttonClearLog.Name = "buttonClearLog";
            this.buttonClearLog.Size = new System.Drawing.Size(75, 23);
            this.buttonClearLog.TabIndex = 15;
            this.buttonClearLog.Text = "Clear";
            this.buttonClearLog.UseVisualStyleBackColor = true;
            this.buttonClearLog.Click += new System.EventHandler(this.buttonClearLog_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 365);
            this.Controls.Add(this.splitContainerVert);
            this.Controls.Add(this.toolStrip);
            this.Name = "MainForm";
            this.Text = "Test Server";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            this.tableLayoutUsers.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton buttonEnabled;
        private System.Windows.Forms.ToolStripButton buttonOpenClient;
        private System.Windows.Forms.SplitContainer splitContainerVert;
        private System.Windows.Forms.ToolStripTextBox textBoxIP;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutUsers;
        private System.Windows.Forms.ListBox listUsers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonClearLog;
        private System.Windows.Forms.Button buttonKickSelected;
        private System.Windows.Forms.ListBox listBoxDebugInfo;
        private System.Windows.Forms.Label label3;
    }
}

