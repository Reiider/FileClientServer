using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FileServer
{
    public partial class Form1 : Form
    {
        bool isClickStartSerer = false;
        AsynchronousIoServer AIServer;
        List<string> ListFiles = new List<string>();
        
        public Form1()
        {
            InitializeComponent();
            //загрузка списка файлов хранащихся на сервере
            IPHostEntry localMachineInfo = Dns.GetHostEntry(Dns.GetHostName());
            tbIP.Text = localMachineInfo.AddressList[1].ToString();
            loadListFiles();
        }

        private void loadListFiles()
        {
            ListFiles.Clear();
            string s = Application.StartupPath + "\\Files";
            try
            {
                string[] list = Directory.GetFiles(s);
                foreach (string str in list)
                {
                    ListFiles.Add(str.Substring(str.LastIndexOf('\\') + 1));
                }
                UpdateList();
            }
            catch
            {
                Directory.CreateDirectory(s);
            }
        }

        private void bStartServer_Click(object sender, EventArgs e)
        {
            if (!isClickStartSerer)
            {
                loadListFiles();
                pInfo.Enabled = false;
                startServer();

                bStartServer.Text = "Stop Server";
                isClickStartSerer = !isClickStartSerer;
            }
            else
            {
                pInfo.Enabled = true;
                stopServer();

                bStartServer.Text = "Start Server";
                isClickStartSerer = !isClickStartSerer;
            }
        }

        private void startServer()
        {
            try
            {

                AIServer = new AsynchronousIoServer(tbIP.Text, int.Parse(tbPort.Text), SynchronizationContext.Current);
                AIServer.updateList += UpdateList;
                AIServer.Ex += Error;
                AIServer.Start(ref ListFiles);
            }
            catch (Exception ex)
            {
                pInfo.Enabled = true;
                stopServer();

                bStartServer.Text = "Start Server";
                isClickStartSerer = !isClickStartSerer;
                MessageBox.Show(ex.Message);
            }
        }

        private void Error(string s)
        {
            MessageBox.Show(s);
        }

        private void stopServer()
        {
            AIServer.Stop();
            AIServer.updateList -= UpdateList;
        }

        private void UpdateList()
        {
            lbInfo.Items.Clear();
            for (int i = 0; i < ListFiles.Count; i++)
            {
                lbInfo.Items.Add(ListFiles[i]);
            }
        }
    }
}
