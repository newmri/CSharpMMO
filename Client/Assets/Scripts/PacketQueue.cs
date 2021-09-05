using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue : MonoBehaviour
{
    public void Push(IPacket packet)
    {
        lock (_lock)
        {
            _packetQueue.Enqueue(packet);
        }
    }

    public IPacket Pop()
    {
        lock (_lock)
        {
            if (0 == _packetQueue.Count)
                return null;

            return _packetQueue.Dequeue();
        }
    }

    public List<IPacket> PopAll()
    {
        List<IPacket> list = new List<IPacket>();

        lock (_lock)
        {
            while (_packetQueue.Count > 0)
            {
                list.Add(_packetQueue.Dequeue());
            }
        }

        return list;
    }

    public static PacketQueue Instance { get; } = new PacketQueue();

    Queue<IPacket> _packetQueue = new Queue<IPacket>();
    object _lock = new object();
}
