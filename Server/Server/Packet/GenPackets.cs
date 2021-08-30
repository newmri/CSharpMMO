using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    CGS_PlayerInfoReq = 1,
	
}

interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

#region CGS_PlayerInfoReq
class CGS_PlayerInfoReq : IPacket
{
    public long playerID;
	public string name;
	
	public class Skill
	{
	    public int id;
		public short level;
		public float duration;
	
	    public void Read(ReadOnlySpan<byte> span, ref ushort count)
	    {
	        this.id = BitConverter.ToInt32(span.Slice(count, span.Length - count));
			count += sizeof(int);
			
			this.level = BitConverter.ToInt16(span.Slice(count, span.Length - count));
			count += sizeof(short);
			
			this.duration = BitConverter.ToSingle(span.Slice(count, span.Length - count));
			count += sizeof(float);
			
	        return;
	    }
	
	    public bool Write(Span<byte> span, ref ushort count)
	    {
	        bool success = true;
	
	        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.id);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.level);
			count += sizeof(short);
			
			success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.duration);
			count += sizeof(float);
			
	        return success;
	    }
	}
	
	public List<Skill> skills = new List<Skill>();

    public ushort Protocol { get { return (ushort)PacketID.CGS_PlayerInfoReq; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);

        this.playerID = BitConverter.ToInt64(span.Slice(count, span.Length - count));
		count += sizeof(long);
		
		ushort nameLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(span.Slice(count, nameLen));
		count += nameLen;
		
		this.skills.Clear();
		ushort skillLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		
		for (int i = 0; i < skillLen; ++i)
		{
		    Skill skill = new Skill();
		    skill.Read(span, ref count);
		    skills.Add(skill);
		}
		 
        return;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.CGS_PlayerInfoReq);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.playerID);
		count += sizeof(long);
		
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLen);
		count += sizeof(ushort);
		count += nameLen;
		
		success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)this.skills.Count);
		count += sizeof(ushort);
		
		foreach (Skill skill in this.skills)
		    success &= skill.Write(span, ref count);
		
        success &= BitConverter.TryWriteBytes(span, count);

        if (!success)
            return null;

        return SendBufferHelper.Close(count);
    }
}
#endregion

