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

            GSC_PlayerList gscPlayerList = new GSC_PlayerList();
            foreach (ClientSession s in _sessions)
            {
                gscPlayerList.players.Add(new GSC_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerID = s.SessionID,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                }) ;
            }

            session.Send(gscPlayerList.Write());

            GSC_BroadcastEnterGame enter = new GSC_BroadcastEnterGame();
            enter.playerID = session.SessionID;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;
            Broadcast(enter.Write());
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Flush()
        {
            foreach (ClientSession clientSession in _sessions)
                clientSession.Send(_pendingList);

            //Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);

            GSC_BroadcastLeaveGame leave = new GSC_BroadcastLeaveGame();
            leave.playerID = session.SessionID;
            Broadcast(leave.Write());
        }

        public void Move(ClientSession session, CGS_Move packet)
        {
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            GSC_BroadcastMove move = new GSC_BroadcastMove();
            move.playerID = session.SessionID;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;
            Broadcast(move.Write());
        }

        List<ClientSession> _sessions = new List<ClientSession>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
    }
}
