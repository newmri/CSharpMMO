using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    GSC_BroadcastEnterGame = 1,
	CGS_LeavGame = 2,
	GSC_BroadcastLeaveGame = 3,
	GSC_PlayerList = 4,
	CGS_Move = 5,
	GSC_BroadcastMove = 6,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

#region GSC_BroadcastEnterGame
public class GSC_BroadcastEnterGame : IPacket
{
    public int playerID;
	public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.GSC_BroadcastEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);

        this.playerID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
        return;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.GSC_BroadcastEnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(this.playerID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
#endregion

#region CGS_LeavGame
public class CGS_LeavGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.CGS_LeavGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);

        
        return;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CGS_LeavGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
#endregion

#region GSC_BroadcastLeaveGame
public class GSC_BroadcastLeaveGame : IPacket
{
    public int playerID;

    public ushort Protocol { get { return (ushort)PacketID.GSC_BroadcastLeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);

        this.playerID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
        return;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.GSC_BroadcastLeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(this.playerID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
#endregion

#region GSC_PlayerList
public class GSC_PlayerList : IPacket
{
    
	public class Player
	{
	    public bool isSelf;
		public int playerID;
		public float posX;
		public float posY;
		public float posZ;
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.isSelf = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			
			this.playerID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
			this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
	        return;
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	
	        Array.Copy(BitConverter.GetBytes(this.isSelf), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			
			Array.Copy(BitConverter.GetBytes(this.playerID), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			
			Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
			Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			
	        return success;
	    }
	}
	
	public List<Player> players = new List<Player>();

    public ushort Protocol { get { return (ushort)PacketID.GSC_PlayerList; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);

        this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		
		for (int i = 0; i < playerLen; ++i)
		{
		    Player player = new Player();
		    player.Read(segment, ref count);
		    players.Add(player);
		}
		 
        return;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.GSC_PlayerList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes((ushort)this.players.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		foreach (Player player in this.players)
		    player.Write(segment, ref count);
		
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
#endregion

#region CGS_Move
public class CGS_Move : IPacket
{
    public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.CGS_Move; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);

        this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
        return;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.CGS_Move), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
#endregion

#region GSC_BroadcastMove
public class GSC_BroadcastMove : IPacket
{
    public int playerID;
	public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.GSC_BroadcastMove; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);

        this.playerID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
        return;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.GSC_BroadcastMove), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(this.playerID), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
#endregion

