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
        _makeFunc.Add((ushort)PacketID.CGS_LeavGame, MakePacket<CGS_LeavGame>);
    _handler.Add((ushort)PacketID.CGS_LeavGame, PacketHandler.CGS_LeavGameHandler);
 _makeFunc.Add((ushort)PacketID.CGS_Move, MakePacket<CGS_Move>);
    _handler.Add((ushort)PacketID.CGS_Move, PacketHandler.CGS_MoveHandler);

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
