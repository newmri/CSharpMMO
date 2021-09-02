using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void CGS_ChatHandler(PacketSession session, IPacket packet)
    {
        CGS_Chat chatPacket = packet as CGS_Chat;
        ClientSession clientSession = session as ClientSession;

        if (null == clientSession.Room)
            return;

        clientSession.Room.Broadcast(clientSession, chatPacket);
    }
}

