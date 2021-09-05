using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class SessionManager
    {
        public static SessionManager Instance { get { return _session; } }
        static SessionManager _session = new SessionManager();

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }

        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (ServerSession session in _sessions)
                {
                    CGS_Move cgsPacket = new CGS_Move();
                    cgsPacket.posX = _rand.Next(-50, 50);
                    cgsPacket.posY = 0.0f;
                    cgsPacket.posZ = _rand.Next(-50, 50);
                    session.Send(cgsPacket.Write());
                }
            }
        }

        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();
        Random _rand = new Random();
    }
}
