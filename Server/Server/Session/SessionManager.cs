using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SessionManager
    {
        public static SessionManager Instance { get { return _session; } }
        static SessionManager _session = new SessionManager();

        public ClientSession Generate()
        {
            lock (_lock)
            {
                int sessionID = ++_sessionID;
                ClientSession session = new ClientSession();
                session.SessionID = sessionID;
                _sessions.Add(sessionID, session);

                Console.WriteLine($"Connected : {sessionID}");
                return session;
            }
        }

        public ClientSession Find(int ID)
        {
            lock (_lock)
            {
                ClientSession session = null;
                _sessions.TryGetValue(ID, out session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionID);
            }
        }

        int _sessionID = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        object _lock = new object();
    }
}
