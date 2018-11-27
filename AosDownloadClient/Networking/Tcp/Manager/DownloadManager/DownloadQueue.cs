using System.Collections.Generic;

namespace AosP2PClient.Networking.Tcp.Manager.DownloadManager
{
    public struct QueueNode
    {
        public readonly int Priority;
        public readonly string FileName;
        public readonly int Count;
        public QueueNode(int priority, string fileName, int count)
        {
            Priority = priority;
            FileName = fileName;
            Count = count;
        }
    }

    public enum DownloadPriority
    {
        High = 3,
        Medium = 2,
        Normal = 1
    }

    public class DownloadQueue
    {
        private Queue<string> HighPriorityQueue;
        private Queue<string> MediumPriorityQueue;
        private Queue<string> NormalPriorityQueue;

        public DownloadQueue()
        {
            HighPriorityQueue = new Queue<string>();
            MediumPriorityQueue = new Queue<string>();
            NormalPriorityQueue = new Queue<string>();
        }

        public void Enqueue(int priority, string fileName)
        {
            if (Contain(fileName))
                return;

            switch (priority)
            {
                case 1:
                    NormalPriorityQueue.Enqueue(fileName);
                    break;
                case 2:
                    MediumPriorityQueue.Enqueue(fileName);
                    break;
                case 3:
                    HighPriorityQueue.Enqueue(fileName);
                    break;
                default:
                    //TODO fix
                    break;
            }
        }

        public QueueNode? Dequeue()
        {
            int count = Count();
            if (HighPriorityQueue.Count > 0)
            {
                return new QueueNode(3, HighPriorityQueue.Dequeue(), count);
            }
            if (MediumPriorityQueue.Count > 0)
            {
                return new QueueNode(2, MediumPriorityQueue.Dequeue(), count);
            }
            if (NormalPriorityQueue.Count > 0)
            {
                return new QueueNode(1, NormalPriorityQueue.Dequeue(), count);
            }
            return null;
        }

        public int Count()
        {
            return HighPriorityQueue.Count + MediumPriorityQueue.Count + NormalPriorityQueue.Count;
        }

        public bool Contain(int priority, string fileName)
        {
            switch (priority)
            {
                case 1:
                    return NormalPriorityQueue.Contains(fileName);
                case 2:
                    return MediumPriorityQueue.Contains(fileName);
                case 3:
                    return HighPriorityQueue.Contains(fileName);
                default:
                    return false;
            }
        }

        public bool Contain(string fileName)
        {
            return NormalPriorityQueue.Contains(fileName) || MediumPriorityQueue.Contains(fileName) || HighPriorityQueue.Contains(fileName);
        }
    }
}
