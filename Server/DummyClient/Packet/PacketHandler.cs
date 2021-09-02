using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void GSC_ChatHandler(PacketSession session, IPacket packet)
    {
        GSC_Chat gscPacket = packet as GSC_Chat;
        ServerSession serverSession = session as ServerSession;

        Console.WriteLine(gscPacket.chat);

    }
}
