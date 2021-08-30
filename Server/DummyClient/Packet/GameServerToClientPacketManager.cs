using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
#region Singleton
public static PacketManager Instance
{
    get
    {
        if (null == _instance)
            _instance = new PacketManager();

        return _instance;
    }
}

static PacketManager _instance;
#endregion

public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
{
    ushort count = 0;
    ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
    count += sizeof(ushort);
    ushort ID = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
    count += sizeof(ushort);

    Action<PacketSession, ArraySegment<byte>> action = null;
    if (_onRecv.TryGetValue(ID, out action))
        action.Invoke(session, buffer);
}

public void Register()
{
   
   return;
}

void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
{
    T packet = new T();
    packet.Read(buffer);

    Action<PacketSession, IPacket> action = null;
    if (_handler.TryGetValue(packet.Protocol, out action))
        action.Invoke(session, packet);
}

Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
}
