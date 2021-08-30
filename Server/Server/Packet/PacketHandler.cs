using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void CGS_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        CGS_PlayerInfoReq p = packet as CGS_PlayerInfoReq;
        Console.WriteLine($"PlayerInfoReq: {p.playerID} {p.name}");

        foreach (CGS_PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"Skill{skill.id} {skill.level} {skill.duration}");
        }
    }
}

