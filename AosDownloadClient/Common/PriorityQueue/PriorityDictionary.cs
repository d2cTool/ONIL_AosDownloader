using System;
using System.Collections.Generic;
using System.Linq;

namespace AosP2PClient.Common.PriorityQueue
{
    public class PriorityDictionary<T> : IPriorityQueue<T>
        where T : IEquatable<T>
    {
        SortedDictionary<int, T> dict = new SortedDictionary<int, T>();
        public PriorityDictionary()
        {
        }

        public int Count => dict.Count;

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(T node)
        {
            return dict.ContainsValue(node);
        }

        public T Dequeue()
        {
            throw new NotImplementedException();
        }

        public void Enqueue(T node, int priority)
        {
            throw new NotImplementedException();
        }

        public void Remove(T node)
        {
            int key = (from d in dict where d.Value.Equals(node) select d).FirstOrDefault().Key;
            //1dict.Select(el => el == node)
        }

        public void UpdatePriority(T node, int priority)
        {
            throw new NotImplementedException();
        }
        
    }
}
