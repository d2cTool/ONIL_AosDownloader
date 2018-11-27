using System;
using System.Linq;

namespace AosP2PClient.Common.PriorityQueue
{
    public class SimplePriorityQueue<TItem> : IPriorityQueue<TItem>
    {
        public int Size { get; private set; }
        private TItem[] nodes;
        private int[] priorities;
        public int Count { get; private set; }

        public SimplePriorityQueue()
        {
            nodes = new TItem[0];
            priorities = new int[0];
            Count = 0;
        }

        public void Enqueue(TItem node, int priority)
        {
            if (Count == 0)
            {
                nodes = nodes.AddAt(Count, node);
                priorities = priorities.AddAt(Count, priority);
                ++Count;
                return;
            }

            if (priority < priorities[0])
            {
                nodes = nodes.AddAt(0, node);
                priorities = priorities.AddAt(0, priority);
                ++Count;
                return;
            }

            if (priority > priorities[Count - 1])
            {
                nodes = nodes.AddAt(Count, node);
                priorities = priorities.AddAt(Count, priority);
                ++Count;
                return;
            }

            int indx = GetInsertionIndex(priority);
            nodes = nodes.AddAt(indx, node);
            priorities = priorities.AddAt(indx, priority);
            ++Count;
        }

        public TItem Dequeue()
        {
            TItem rez = nodes[0];
            Remove(0);
            return rez;
        }

        public void Clear()
        {
            Array.Clear(nodes, 0, Count);
            Array.Clear(priorities, 0, Count);
            Count = 0;
        }

        public bool Contains(TItem node)
        {
            if (Count > 0)
            {
                return nodes.Contains(node);
            }
            return false;
        }

        public void Remove(TItem node)
        {
            int index = Array.IndexOf(nodes, node);
            if (index != -1)
            {
                Remove(index);
            }
        }

        public void Remove(int index)
        {
            nodes = nodes.RemoveAt(index);
            priorities = priorities.RemoveAt(index);
            --Count;
        }

        public void UpdatePriority(TItem node, int priority)
        {
            Remove(node);
            Enqueue(node, priority);
        }

        private int GetInsertionIndex(int priority)
        {
            if (priority > priorities[Count - 1])
            {
                return Count;
            }

            int index = Array.BinarySearch(priorities, priority);

            if (index > -1)
            {
                return index;
            }

            int nearest = priorities.First(t => (t - priority) > 0);
            int i = Array.IndexOf(priorities, nearest);
            return (i > -1) ? i : Count;
        }
    }
}
