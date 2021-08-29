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
    class PlayerInfoReq
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

        public void Read(ArraySegment<byte> segment)
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

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> span = new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.PlayerInfoReq);
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

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            PlayerInfoReq packet = new PlayerInfoReq() { _playerID = 1001, _name = "ABCD" };
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { _id = 101, _level = 1, _duration = 3.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { _id = 102, _level = 2, _duration = 4.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { _id = 103, _level = 3, _duration = 5.0f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { _id = 104, _level = 4, _duration = 6.0f });

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
