using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace DummyClient
{
    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            CGS_PlayerInfoReq packet = new CGS_PlayerInfoReq() { playerID = 1001, name = "ABCD" };
            packet.skills.Add(new CGS_PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f });
            packet.skills.Add(new CGS_PlayerInfoReq.Skill() { id = 102, level = 2, duration = 4.0f });
            packet.skills.Add(new CGS_PlayerInfoReq.Skill() { id = 103, level = 3, duration = 5.0f });
            packet.skills.Add(new CGS_PlayerInfoReq.Skill() { id = 104, level = 4, duration = 6.0f });

            ArraySegment<byte> segment = packet.Write();
            if (null!= segment)
                Send(segment);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {

        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {

        }
    }
}
