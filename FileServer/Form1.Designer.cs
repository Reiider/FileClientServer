namespace FileServer
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbIP = new System.Windows.Forms.TextBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pInfo = new System.Windows.Forms.Panel();
            this.bStartServer = new System.Windows.Forms.Button();
            this.lbInfo = new System.Windows.Forms.ListBox();
            this.pInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // tbIP
            // 
            this.tbIP.Location = new System.Drawing.Point(26, 8);
            this.tbIP.Name = "tbIP";
            this.tbIP.Size = new System.Drawing.Size(118, 20);
            this.tbIP.TabIndex = 1;
            this.tbIP.Text = "127.0.0.1";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(208, 8);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(83, 20);
            this.tbPort.TabIndex = 2;
            this.tbPort.Text = "8998";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port";
            // 
            // pInfo
            // 
            this.pInfo.Controls.Add(this.label1);
            this.pInfo.Controls.Add(this.tbPort);
            this.pInfo.Controls.Add(this.label2);
            this.pInfo.Controls.Add(this.tbIP);
            this.pInfo.Location = new System.Drawing.Point(8, 12);
            this.pInfo.Name = "pInfo";
            this.pInfo.Size = new System.Drawing.Size(303, 36);
            this.pInfo.TabIndex = 4;
            // 
            // bStartServer
            // 
            this.bStartServer.Location = new System.Drawing.Point(61, 292);
            this.bStartServer.Name = "bStartServer";
            this.bStartServer.Size = new System.Drawing.Size(199, 24);
            this.bStartServer.TabIndex = 6;
            this.bStartServer.Text = "Start Server";
            this.bStartServer.UseVisualStyleBackColor = true;
            this.bStartServer.Click += new System.EventHandler(this.bStartServer_Click);
            // 
            // lbInfo
            // 
            this.lbInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbInfo.FormattingEnabled = true;
            this.lbInfo.Location = new System.Drawing.Point(9, 57);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbInfo.Size = new System.Drawing.Size(301, 223);
            this.lbInfo.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 322);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.bStartServer);
            this.Controls.Add(this.pInfo);
            this.MaximumSize = new System.Drawing.Size(339, 360);
            this.MinimumSize = new System.Drawing.Size(339, 360);
            this.Name = "Form1";
            this.Text = "FileServer";
            this.pInfo.ResumeLayout(false);
            this.pInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbIP;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pInfo;
        private System.Windows.Forms.Button bStartServer;
        private System.Windows.Forms.ListBox lbInfo;
    }
}

