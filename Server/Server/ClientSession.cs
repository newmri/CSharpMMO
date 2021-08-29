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
    public abstract class Packet
    {
        public ushort _size;
        public ushort _packetID;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> segment);

    }

    class PlayerInfoReq : Packet
    {
        public long _playerID;
        public string _name;

        public struct SkillInfo
        {
            public int _id;
            public short _level;
            public float _duration;

            public bool Write(Span<byte> span, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), _id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), _level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), _duration);
                count += sizeof(float);

                return success;
            }

            public void Read(ReadOnlySpan<byte> span, ref ushort count)
            {
                _id = BitConverter.ToInt32(span.Slice(count, span.Length - count));
                count += sizeof(int);
                _level = BitConverter.ToInt16(span.Slice(count, span.Length - count));
                count += sizeof(short);
                _duration = BitConverter.ToSingle(span.Slice(count, span.Length - count));
                count += sizeof(float);
            }
        }

        public List<SkillInfo> skills = new List<SkillInfo>();

        public PlayerInfoReq()
        {
            _packetID = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            count += sizeof(ushort);

            _playerID = BitConverter.ToInt64(span.Slice(count, span.Length - count));
            count += sizeof(long);

            ushort nameLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
            count += sizeof(ushort);
            _name = Encoding.Unicode.GetString(span.Slice(count, nameLen));
            count += nameLen;

            skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
            count += sizeof(ushort);

            for (int i = 0; i < skillLen; ++i)
            {
                SkillInfo skill = new SkillInfo();
                skill.Read(span, ref count);
                skills.Add(skill);
            }
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> span = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), _packetID);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), _playerID);
            count += sizeof(long);

            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(_name, 0, _name.Length, openSegment.Array, openSegment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;

            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);

            foreach (SkillInfo skill in skills)
                success &= skill.Write(span, ref count);

            success &= BitConverter.TryWriteBytes(span, count);

            if (!success)
                return null;

            return SendBufferHelper.Close(count);
        }
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
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"PlayerInfoReq: {p._playerID} {p._name}");

                        foreach (PlayerInfoReq.SkillInfo skill in p.skills)
                        {
                            Console.WriteLine($"Skill{skill._id} {skill._level} {skill._duration}");
                        }
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
