using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Packet
    {
        public ushort size;
        public ushort packetID;
    }

    class PlayerInfoReq : Packet
    {
        public long playerID;
    }

    class PlayerInfoOK : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOK = 2
    }

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
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            ushort ID = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketID)ID)
            {
                case PacketID.PlayerInfoReq:
                    {
                        long playerID = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                        count += 8;
                        Console.WriteLine($"PlayerInfoReq: {playerID}");
                    }
                    break;
            }

            Console.WriteLine($"RecvPacketID: {ID} size: {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {

        }

        public override void OnSend(int numOfBytes)
        {

        }
    }

}
