using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void CGS_LeavGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (null == clientSession.Room)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));   
    }

    public static void CGS_MoveHandler(PacketSession session, IPacket packet)
    {
        CGS_Move movePacket = packet as CGS_Move;
        ClientSession clientSession = session as ClientSession;

        if (null == clientSession.Room)
            return;

        //Console.WriteLine($"{movePacket.posX}, {movePacket.posY}, {movePacket.posZ}");

        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, movePacket));
    }
}

