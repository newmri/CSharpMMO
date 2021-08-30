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
            
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length);
            Send(sendBuff);

            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnRevPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {

        }

        public override void OnSend(int numOfBytes)
        {

        }
    }

}
