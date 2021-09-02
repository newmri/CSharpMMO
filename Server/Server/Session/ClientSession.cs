using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {

            Console.WriteLine($"OnConnected: {endPoint}");
            Program.Room.Enter(this);
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
            
            SessionManager.Instance.Remove(this);
            if (null != Room)
            {
                Room.Leave(this);
                Room = null;
            }
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Sent bytes: {numOfBytes}");
        }

        public int SessionID { get; set; }
        public GameRoom Room { get; set; }
    }

}
