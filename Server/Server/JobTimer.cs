using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int CompareTo(JobTimerElem other)
        {
            return other._execTick - _execTick;
        }

        public int _execTick;
        public Action _action;
    }

    class JobTimer
    {
        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerElem job;
            job._execTick = System.Environment.TickCount + tickAfter;
            job._action = action;

            lock (_lock)
            {
                priorityQueue.Push(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;

                JobTimerElem job;

                lock (_lock)
                {
                    if (0 == priorityQueue.Count)
                        break;

                    job = priorityQueue.Peek();
                    if (job._execTick > now)
                        break;

                    priorityQueue.Pop();
                }

                job._action.Invoke();
            }
        }

        public static JobTimer Instance { get; } = new JobTimer();

        PriorityQueue<JobTimerElem> priorityQueue = new PriorityQueue<JobTimerElem>();
        object _lock = new object();
    }
}
