namespace AosP2PClient.Common.PriorityQueue
{
    public interface IPriorityQueue<TItem>
    {
        void Enqueue(TItem node, int priority);
        TItem Dequeue();
        void Clear();
        bool Contains(TItem node);
        void Remove(TItem node);
        void UpdatePriority(TItem node, int priority);
        int Count { get; }
    }
}
