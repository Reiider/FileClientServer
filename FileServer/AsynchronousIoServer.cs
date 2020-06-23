using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace FileServer
{
    class AsynchronousIoServer
    {
        private SynchronizationContext _sc;

        public delegate void mess();
        public event mess updateList;

        public delegate void Exept(string s);
        public event Exept Ex;

        private Socket _serverSocket;
        private string _ip;
        private int _port;

        private List<string> ListFiles;
        private List<ConnectionInfo> _connections = new List<ConnectionInfo>();//список подключений (клиентов)
        

        public AsynchronousIoServer(string ip, int port, SynchronizationContext sc) 
        {
            _ip = ip;
            _port = port;
            _sc = sc;
        }

        private void Updating(object o)
        {
            updateList();
        }

        private void SetupServerSocket()
        {
            // Получаем информацию о локальном компьютере
            IPEndPoint myEndpoint = new IPEndPoint(IPAddress.Parse(_ip), _port);

            // Создаем сокет, привязываем его к адресу
            // и начинаем прослушивание
            _serverSocket = new Socket(myEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(myEndpoint);
            _serverSocket.Listen((int) SocketOptionName.MaxConnections);
        }

        private class ConnectionInfo
        {
            public Socket Socket;
            public byte[] Buffer; // для передачи файла
            public string FileName; //имя передаваемого файла (для информирования клиентов)
            public bool isStarted = false; //указывает на: передача фала не начиналась, или уже началась
            public FileStream FileStream; //поток для записи файла
            public long liftSize; //количество байт которые осталось получить
            public MemoryStream ms = new MemoryStream(); //временное хранение заголовка передаваемого файла
        }

        public void Start(ref List<string> ls)
        {
            ListFiles = ls;
            SetupServerSocket();
            for (int i = 0; i < 10; i++) // 10 запросов могут обрабатываться одновременно
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), _serverSocket);    
        }

        private void AcceptCallback(IAsyncResult result)
        {
            ConnectionInfo connection = new ConnectionInfo();
            try
            {
                // Завершение операции Accept
                Socket s = (Socket)result.AsyncState;
                connection.Socket = s.EndAccept(result);
                connection.Buffer = new byte[255];
                lock (_connections) _connections.Add(connection);
                //отправить клиенту список файлов
                string lFiles = "";
                foreach (string str in ListFiles)
                {
                    lFiles += str + "|";
                }
                byte[] b= Encoding.Unicode.GetBytes(lFiles);
                b = Encoding.Unicode.GetBytes(b.Length.ToString() + "|" + lFiles);
                connection.Socket.Send(b);

                // Начало операции Receive и новой операции Accept
                connection.Socket.BeginReceive(
                    connection.Buffer, 
                    0, 
                    connection.Buffer.Length, 
                    SocketFlags.None,
                    new AsyncCallback(ReceiveCallback),
                    connection);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), result.AsyncState);
            }
            catch
            {
                CloseConnection(connection);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = (ConnectionInfo)result.AsyncState;
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
                        if (str[0] == "Download" && str.Length == 3)
                        {
                            FileInfo fi = new FileInfo("Files\\" + str[1]);
                            if (fi.Exists)
                            {
                                byte[] data = Encoding.Unicode.GetBytes("File" + "|" + str[1] + "|" + fi.Length.ToString() + "|");
                                connection.Socket.SendFile("Files\\" + str[1], data, null, TransmitFileOptions.UseDefaultWorkerThread);// (data);
                            }
                            else connection.Socket.Send(Encoding.Unicode.GetBytes("Error|Файл не найден."));
                            connection.ms.Close();
                            connection.ms = new MemoryStream();
                        }
                        else if(str.Length == 4 && str[0] == "File")
                        {
                            string name = str[1];
                            if (str[1].Length >= 150)
                            {
                                name = str[1].Substring(0, 120);
                                name += " (ErrorFullName)";
                                name += str[1].Substring(str[1].LastIndexOf("."));
                            }
                            FileInfo fi = new FileInfo("Files\\"+name);
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
                            connection.FileName = name;
                            connection.FileStream = new FileStream("Files\\" + name, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);

                            string s = temp.Substring(temp.LastIndexOf('|') + 1);
                            byte[] tempB = Encoding.Unicode.GetBytes(s);
                            if (bytesRead != 255)
                            {
                                connection.FileStream.Write(tempB, 0, tempB.Length);// -1 потому что при преобразовании обратно из строки в байты появляется байт конца строки
                                connection.liftSize -= (tempB.Length);
                            }
                            else
                            {
                                connection.FileStream.Write(tempB, 0, tempB.Length - 1);// -1 потому что при преобразовании обратно из строки в байты появляется байт конца строки
                                connection.liftSize -= (tempB.Length - 1);
                            }
                            connection.ms.Close();
                            connection.ms = new MemoryStream();
                            if (connection.liftSize == 0) //конец передачи
                            {
                                connection.isStarted = false;
                                connection.FileStream.Close();
                                ListFiles.Add(connection.FileName);
                                _sc.Post(Updating, null);
                                //проинформировать подключенных клиентов
                                lock (_connections)
                                {
                                    foreach (ConnectionInfo conn in _connections)
                                    {
                                        try
                                        {
                                            conn.Socket.Send(Encoding.Unicode.GetBytes(connection.FileName + "|")); //пресылка сообщения всем
                                        }
                                        catch 
                                        {
                                            CloseConnection(conn);
                                        }
                                    }
                                }
                            }
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
                            ListFiles.Add(connection.FileName);
                            _sc.Post(Updating, null);
                            //проинформировать подключенных клиентов
                            lock (_connections)
                            {
                                foreach (ConnectionInfo conn in _connections) conn.Socket.Send(Encoding.Unicode.GetBytes(connection.FileName + "|")); //пресылка сообщения всем, ктоме того кто прислал сообщение
                            }
                        }
                    }
                }
            }
            catch {}

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
                CloseConnection(connection);
            } 
        }

        private void CloseConnection(ConnectionInfo ci)
        {
            try
            {
                ci.Socket.Close();
                lock (_connections) _connections.Remove(ci);
            }
            catch { }
        }

        public void Stop()
        {
            for(int i = _connections.Count - 1; i >= 0 ; i--)
            {
                CloseConnection(_connections[i]);
            }
            _serverSocket.Close(1);
        }
    }
}
