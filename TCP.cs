using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BattleToad.TCP
{
    public class TCPServer : IDisposable
    {
        public readonly int Port;
        private readonly Thread ListenThread;
        private TcpListener tcpListener;
        public TCPServer(int port)
        {
            Port = port;
            ListenThread = new Thread(() => Listener());
            ListenThread.IsBackground = true;
            ListenThread.Start();
        }
        ~TCPServer()
        {
            Dispose(false);
        }

        private void Listener()
        {
            IPAddress ipAddr = IPAddress.Any;
            tcpListener = new TcpListener(ipAddr, Port);
            tcpListener.Start();
            while (true)
            {
                Socket s = tcpListener.AcceptSocket();
                Task.Run(() => ReadSocket(s));
            }
        }

        private void ReadSocket(Socket socket)
        {
            byte[] b = new byte[10000];
            int k = -1;
            string userIP = null;
            int userPort = -1;
            string bufStr;
            StringBuilder RecievedData = new StringBuilder();
            socket.SendTimeout = 200;
            do
            {
                k = socket.Receive(b);
                if (k == 0) continue;
                userIP = socket.RemoteEndPoint.ToString().Split(':')[0];
                int.TryParse(socket.RemoteEndPoint.ToString().Split(':')[1], out userPort);
                bufStr = Encoding.Unicode.GetString(b.Take(k).ToArray());
                RecievedData.Append(bufStr);
                Console.WriteLine($"k={k}\nbuf={bufStr}");
                if (RecievedData.Length != 0 && userIP != null && userPort > 0) Recieved(RecievedData.ToString(), userIP, userPort);
            }
            while (socket.Connected);
            socket.Dispose();
        }

        private void Recieved(string Message, string IP, int Port)
        {
            Console.WriteLine(Message, IP);
            Task.Run(() => SendData(IP, Port, "Hello World"));
        }

        public void SendData(string IP, int Port, string SString)
        {
            try
            {
                Console.WriteLine($"Отправка {IP}:{Port}");
                TcpClient tcpclnt = new TcpClient();
                tcpclnt.Connect(IP, Port);
                Stream stm = tcpclnt.GetStream();
                byte[] ba = Encoding.Unicode.GetBytes(SString);
                stm.Write(ba, 0, ba.Length);
                tcpclnt.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки {ex.Message}");
            }
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ListenThread.Abort();
                    tcpListener.Stop();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}