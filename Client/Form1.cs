using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Client
{

    public partial class Form1 : Form
    {
        List<OneFile> listElems = new List<OneFile>(); //список элементов на панели (соответсвует списку файлов)
        List<string> listFiles = new List<string>(); //список файлов на сервере

        bool isClickConnect = false;
        ConnectionInfo connect;

        SynchronizationContext sc;

        public delegate void s(object o);
        public Form1()
        {
            InitializeComponent();
            pListFiles.Enabled = false;
            bAddFile.Enabled = false;
            sc = SynchronizationContext.Current;
            pListFiles.AutoScroll = true;
            
            string s = Application.StartupPath + "\\Files";
            Directory.CreateDirectory(s);

            IPHostEntry localMachineInfo = Dns.GetHostEntry(Dns.GetHostName());
            tbIP.Text = localMachineInfo.AddressList[1].ToString();
            
        }

        private class ConnectionInfo
        {
            public Socket Socket;
            public byte[] Buffer; // для передачи файла
            public bool isStarted = false; //указывает на: передача фала не начиналась, или уже началась
            public FileStream FileStream; //поток для записи файла
            public long liftSize; //количество байт которые осталось получить
            public MemoryStream ms = new MemoryStream();
        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            if (!isClickConnect)
            {
                pInfo.Enabled = false;
                pListFiles.Enabled = true;
                bAddFile.Enabled = true;
                Connect();

                bConnect.Text = "Disconnect";
                isClickConnect = !isClickConnect;
            }
            else
            {
                pInfo.Enabled = true;
                pListFiles.Enabled = false;
                bAddFile.Enabled = false;
                Disconnect();

                bConnect.Text = "Connect";
                isClickConnect = !isClickConnect;
            }
        }

        private void Connect()
        {
            connect = new ConnectionInfo();
            try
            {
                IPEndPoint myEndpoint = new IPEndPoint(IPAddress.Parse(tbIP.Text), int.Parse(tbPort.Text));
                connect.Socket = new Socket(myEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                connect.Buffer = new byte[255];
                connect.Socket.BeginConnect(myEndpoint, connected, connect);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                pInfo.Enabled = true;
                pListFiles.Enabled = false;
                bAddFile.Enabled = false;

                bConnect.Text = "Connect";
                isClickConnect = false;
            }
        }

        private void connected(IAsyncResult result)
        {
            ConnectionInfo connection = connect = (ConnectionInfo)result.AsyncState;
            try
            {
                int readedBytes;
                string listF = "";
                string[] sMassAccessFile;
                MemoryStream ms = new MemoryStream();
                do
                {
                    readedBytes = connection.Socket.Receive(connection.Buffer);
                    ms.Write(connection.Buffer, 0, readedBytes);
                    listF += Encoding.Unicode.GetString(connection.Buffer, 0, readedBytes);
                    sMassAccessFile = listF.Split('|');
                }
                while (sMassAccessFile.Length == 1); //считываем до тех пор, пока строка с размером байт принимаемого списка не сформируется
                int countBytes = int.Parse(sMassAccessFile[0]); //количество бит которое нужно принять
                string RecivingList = listF.Substring(listF.IndexOf('|') + 1);
                byte[] tempB = Encoding.Unicode.GetBytes(RecivingList);
                if (countBytes <= 255) countBytes -= (tempB.Length); // -1 потому что при преобразовании обратно из строки в байты появляется байт конца строки
                else countBytes -= (tempB.Length - 1);
                while (countBytes != 0)
                {
                    readedBytes = connection.Socket.Receive(connection.Buffer);
                    ms.Write(connection.Buffer, 0, readedBytes);
                    countBytes -= readedBytes;
                }
                listF = Encoding.Unicode.GetString(ms.GetBuffer());
                sMassAccessFile = listF.Split('|');
                listFiles.Clear();
                for (int i = 1; i < sMassAccessFile.Length - 1; i++) //от 1, тк в первом записано кол-во байт     -1, так как в последнем будет записан мусор
                {
                    listFiles.Add(sMassAccessFile[i]);
                }
                sc.Post(Updating, null);

                connection.Socket.BeginReceive(
                    connection.Buffer,
                    0,
                    connection.Buffer.Length,
                    SocketFlags.None,
                    new AsyncCallback(ReceiveCallback),
                    connection);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                sc.Post(ServerStoped, null);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = connect = (ConnectionInfo)result.AsyncState;
            try
            {
                int bytesRead = connection.Socket.EndReceive(result);
                if (bytesRead != 0)
                {
                    if (!connection.isStarted)
                    {
                        connection.ms.Write(connection.Buffer, 0, bytesRead);
                        string temp = Encoding.Unicode.GetString(connection.ms.GetBuffer(), 0, (int) connection.ms.Length);
                        string[] str = temp.Split('|');
                        if (str[0] == "Error")
                        {
                            MessageBox.Show(str[1]);
                            connection.ms.Close();
                            connection.ms = new MemoryStream();
                        }
                        else if (str.Length == 4 && str[0] == "File")
                        {
                            string name = str[1];
                            if (str[1].Length >= 150)
                            {
                                name = str[1].Substring(0, 120);
                                name += " (ErrorFullName)";
                                name += str[1].Substring(str[1].LastIndexOf("."));
                            }
                            FileInfo fi = new FileInfo("Files\\" + name);
                            if (fi.Exists) //если файл существует назначаем ему новое имя
                            {
                                string lastName = name;
                                for (int i = 1; ; i++)
                                {
                                    name = lastName.Substring(0, lastName.LastIndexOf('.')) + " (" + i.ToString() + ")" + lastName.Substring(lastName.LastIndexOf('.'));
                                    fi = new FileInfo("Files\\" + name);
                                    if (!fi.Exists) break;
                                }
                            }
                            //Передача файла с именем name
                            connection.liftSize = long.Parse(str[2]);
                            connection.isStarted = true;
                            connection.FileStream = new FileStream("Files\\" + name, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);

                            byte[] tempB = Encoding.Unicode.GetBytes(temp.Substring(temp.LastIndexOf('|') + 1));
                            if (bytesRead != 255)
                            {
                                connection.FileStream.Write(tempB, 0, tempB.Length); 
                                connection.liftSize -= (tempB.Length);
                            }
                            else
                            {
                                connection.FileStream.Write(tempB, 0, tempB.Length - 1); // -1 потому что при преобразовании обратно из строки в байты появляется байт конца строки
                                connection.liftSize -= (tempB.Length - 1);
                            }
                            connection.ms.Close();
                            connection.ms = new MemoryStream();
                            sc.Post(setEnabledPanelFile, false);
                            if (connection.liftSize == 0) //конец передачи
                            {
                                connection.isStarted = false;
                                connection.FileStream.Close();
                                MessageBox.Show("Файл загружен.");
                                sc.Post(setEnabledPanelFile, true);
                            }
                        }
                        else if (str.Length == 2 && str[0] != "File")
                        {
                            listFiles.Add(str[0]);
                            sc.Post(Updating, null);
                            connection.ms.Close();
                            connection.ms = new MemoryStream();
                        }
                    }
                    else
                    {
                        //Продолжение передачи
                        connection.FileStream.Write(connection.Buffer, 0, bytesRead);
                        connection.liftSize -= bytesRead;
                        if (connection.liftSize == 0) //конец передачи
                        {
                            connection.isStarted = false;
                            connection.FileStream.Close();
                            MessageBox.Show("Файл загружен.");
                            sc.Post(setEnabledPanelFile, true);
                        }
                    } 
                }
            }
            catch { }
            try
            {
                connection.Socket.BeginReceive(
                            connection.Buffer,
                            0,
                            connection.Buffer.Length,
                            SocketFlags.None,
                            new AsyncCallback(ReceiveCallback),
                            connection);
            }
            catch 
            {
                connection.Socket.Close(1);
                MessageBox.Show("Сервер недоступен.");
                sc.Post(ServerStoped, null);
            }
        }

        private void ServerStoped(object o)
        {
            pInfo.Enabled = true;
            pListFiles.Enabled = false;
            bAddFile.Enabled = false;

            bConnect.Text = "Connect";
            isClickConnect = false;
        }

        private void Disconnect()
        {
            connect.Socket.Close();
            sc.Post(Updating, null);
        }

        private void bAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog();
            if (OPF.ShowDialog() == DialogResult.OK)
            {
                string file = OPF.FileName;
                FileInfo fi = new FileInfo(file);
                int find = file.LastIndexOf('\\');
                string s = file.Substring(find + 1, file.Length - find - 1);

                byte[] data = Encoding.Unicode.GetBytes("File|"+s+"|"+fi.Length.ToString()+"|");
                try
                {
                    connect.Socket.SendFile(file, data, null, TransmitFileOptions.UseDefaultWorkerThread);// (data);
                }
                catch { MessageBox.Show("Ошибка при передачи файла"); }
            }
        }

        private void Updating(object state)
        {
            pListFiles.Controls.Clear();
            listElems.Clear();
            for (int i = 0; i < listFiles.Count; i++)
            {
                OneFile of = new OneFile(); 
                of.set(listFiles[i], i);
                of.Download += DownloadFile;
                of.Location = new Point(0, 22 * i - pInfo.VerticalScroll.Value);
                listElems.Add(of);
                pListFiles.Controls.Add(of);
            }
        }

        private void DownloadFile(int i)
        {
            byte[] b = Encoding.Unicode.GetBytes("Download|"+listFiles[i]+"|");
            connect.Socket.Send(b);
        }

        private void setEnabledPanelFile(object state)
        {
            bool b = (bool)state;
            pListFiles.Enabled = b;
        }
    }
}