using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
    #region Singleton
    public static PacketManager Instance { get { return _session; } }
    static PacketManager _session = new PacketManager();
    #endregion

    PacketManager()
    {
        Register();
    }

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
    _onRecv.Add((ushort)PacketID.CGS_Chat, MakePacket<CGS_Chat>);
    _handler.Add((ushort)PacketID.CGS_Chat, PacketHandler.CGS_ChatHandler);

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
