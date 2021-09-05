using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void GSC_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        GSC_BroadcastEnterGame gscPacket = packet as GSC_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;
    }
    public static void GSC_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        GSC_BroadcastLeaveGame gscPacket = packet as GSC_BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;
    }
    public static void GSC_PlayerListHandler(PacketSession session, IPacket packet)
    {
        GSC_PlayerList gscPacket = packet as GSC_PlayerList;
        ServerSession serverSession = session as ServerSession;
    }
    public static void GSC_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        GSC_BroadcastMove gscPacket = packet as GSC_BroadcastMove;
        ServerSession serverSession = session as ServerSession;
    }
}
