using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
    #region Singleton
    public static PacketManager Instance { get { return _session; } }
    static PacketManager _session = new PacketManager();
    #endregion

    PacketManager()
    {
        Register();
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);
        ushort ID = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;

        if (_makeFunc.TryGetValue(ID, out func))
        {
            IPacket packet = func.Invoke(session, buffer);
            if (null != onRecvCallback)
                onRecvCallback.Invoke(session, packet);
            else
                HandlerPacket(session, packet);
        }
    }

    public void Register()
    {
        _makeFunc.Add((ushort)PacketID.GSC_BroadcastEnterGame, MakePacket<GSC_BroadcastEnterGame>);
    _handler.Add((ushort)PacketID.GSC_BroadcastEnterGame, PacketHandler.GSC_BroadcastEnterGameHandler);
 _makeFunc.Add((ushort)PacketID.GSC_BroadcastLeaveGame, MakePacket<GSC_BroadcastLeaveGame>);
    _handler.Add((ushort)PacketID.GSC_BroadcastLeaveGame, PacketHandler.GSC_BroadcastLeaveGameHandler);
 _makeFunc.Add((ushort)PacketID.GSC_PlayerList, MakePacket<GSC_PlayerList>);
    _handler.Add((ushort)PacketID.GSC_PlayerList, PacketHandler.GSC_PlayerListHandler);
 _makeFunc.Add((ushort)PacketID.GSC_BroadcastMove, MakePacket<GSC_BroadcastMove>);
    _handler.Add((ushort)PacketID.GSC_BroadcastMove, PacketHandler.GSC_BroadcastMoveHandler);

       return;
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read(buffer);
        return packet;
    }

    public void HandlerPacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);
    }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
}
