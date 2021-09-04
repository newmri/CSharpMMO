using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening....");

            FlushRoom();

            while (true)
            {
                JobTimer.Instance.Flush();
            }

        }

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();
    }
}
