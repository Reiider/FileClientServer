namespace Client
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
            this.pInfo = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbIP = new System.Windows.Forms.TextBox();
            this.pListFiles = new System.Windows.Forms.Panel();
            this.bConnect = new System.Windows.Forms.Button();
            this.bAddFile = new System.Windows.Forms.Button();
            this.pInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pInfo
            // 
            this.pInfo.Controls.Add(this.label1);
            this.pInfo.Controls.Add(this.tbPort);
            this.pInfo.Controls.Add(this.label2);
            this.pInfo.Controls.Add(this.tbIP);
            this.pInfo.Location = new System.Drawing.Point(12, 12);
            this.pInfo.Name = "pInfo";
            this.pInfo.Size = new System.Drawing.Size(347, 36);
            this.pInfo.TabIndex = 5;
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
            // tbIP
            // 
            this.tbIP.Location = new System.Drawing.Point(26, 8);
            this.tbIP.Name = "tbIP";
            this.tbIP.Size = new System.Drawing.Size(118, 20);
            this.tbIP.TabIndex = 1;
            this.tbIP.Text = "127.0.0.1";
            // 
            // pListFiles
            // 
            this.pListFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pListFiles.Location = new System.Drawing.Point(12, 57);
            this.pListFiles.Name = "pListFiles";
            this.pListFiles.Size = new System.Drawing.Size(347, 233);
            this.pListFiles.TabIndex = 6;
            // 
            // bConnect
            // 
            this.bConnect.Location = new System.Drawing.Point(174, 307);
            this.bConnect.Name = "bConnect";
            this.bConnect.Size = new System.Drawing.Size(185, 21);
            this.bConnect.TabIndex = 7;
            this.bConnect.Text = "Connect";
            this.bConnect.UseVisualStyleBackColor = true;
            this.bConnect.Click += new System.EventHandler(this.bConnect_Click);
            // 
            // bAddFile
            // 
            this.bAddFile.Location = new System.Drawing.Point(12, 305);
            this.bAddFile.Name = "bAddFile";
            this.bAddFile.Size = new System.Drawing.Size(75, 23);
            this.bAddFile.TabIndex = 8;
            this.bAddFile.Text = "Add File";
            this.bAddFile.UseVisualStyleBackColor = true;
            this.bAddFile.Click += new System.EventHandler(this.bAddFile_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 336);
            this.Controls.Add(this.bAddFile);
            this.Controls.Add(this.bConnect);
            this.Controls.Add(this.pListFiles);
            this.Controls.Add(this.pInfo);
            this.MaximumSize = new System.Drawing.Size(387, 374);
            this.MinimumSize = new System.Drawing.Size(387, 374);
            this.Name = "Form1";
            this.Text = "Client";
            this.pInfo.ResumeLayout(false);
            this.pInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbIP;
        private System.Windows.Forms.Panel pListFiles;
        private System.Windows.Forms.Button bConnect;
        private System.Windows.Forms.Button bAddFile;
    }
}

