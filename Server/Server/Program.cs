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
        class GameSession : Session
        {
            public override void OnConnected(EndPoint endPoint)
            {
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
                Send(sendBuff);
                Thread.Sleep(1000);
                Disconnect();
            }

            public override void OnDisconnected(EndPoint endPoint)
            {

            }

            public override int OnRecv(ArraySegment<byte> buffer)
            {
                string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                Console.WriteLine($"[From Client] {recvData}");
                return buffer.Count;
            }

            public override void OnSend(int numOfBytes)
            {

            }
        }

        static void Main(string[] args)
        {
            // DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening....");

            while (true)
            {
                ;
            }

        }

        static Listener _listener = new Listener();
    }
}
