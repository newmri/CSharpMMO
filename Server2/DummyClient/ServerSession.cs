using System;
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

        public override void OnConnected(EndPoint endPoint)
        {
            PlayerInfoReq packet = new PlayerInfoReq() { size = 4, packetID = (ushort)PacketID.PlayerInfoReq, playerID = 1001 };

            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            bool success = true;

            success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count), packet.size);


            byte[] size = BitConverter.GetBytes(packet.size);
            byte[] packetID = BitConverter.GetBytes(packet.packetID);
            byte[] playerID = BitConverter.GetBytes(packet.playerID);

            ushort count = 0;
            Array.Copy(size, 0, openSegment.Array, openSegment.Offset + count, size.Length);
            count += 2;
            Array.Copy(packetID, 0, openSegment.Array, openSegment.Offset + count, packetID.Length);
            count += 2;
            Array.Copy(playerID, 0, openSegment.Array, openSegment.Offset + count, playerID.Length);
            count += 8;
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);
            Send(sendBuff);
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
