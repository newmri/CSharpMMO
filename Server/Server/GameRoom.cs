using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameRoom
    {
        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Broadcast(ClientSession session, CGS_Chat cgsPacket)
        {
            GSC_Chat gscPacket = new GSC_Chat();
            gscPacket.playerID = session.SessionID;
            gscPacket.chat = cgsPacket.chat + $" From {gscPacket.playerID}";
            ArraySegment<byte> segment = gscPacket.Write();

            lock (_lock)
            {
                foreach (ClientSession clientSession in _sessions)
                    clientSession.Send(segment);
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }

        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();
    }
}
