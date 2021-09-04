using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        JobQueue _jobQueue = new JobQueue();

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
        }

        public void Broadcast(ClientSession session, CGS_Chat cgsPacket)
        {
            GSC_Chat gscPacket = new GSC_Chat();
            gscPacket.playerID = session.SessionID;
            gscPacket.chat = cgsPacket.chat + $" From {gscPacket.playerID}";
            ArraySegment<byte> segment = gscPacket.Write();

            _pendingList.Add(segment);
        }

        public void Flush()
        {
            foreach (ClientSession clientSession in _sessions)
                clientSession.Send(_pendingList);

            Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
        }

        List<ClientSession> _sessions = new List<ClientSession>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
    }
}
