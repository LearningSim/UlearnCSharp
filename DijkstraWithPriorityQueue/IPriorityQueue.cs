namespace DijkstraWithPriorityQueue;

public interface IPriorityQueue<TValue>
{
    (bool Success, double Priority) Peek(TValue value);
    void Add(TValue value, double priority);
    void Remove(TValue value);
    void Update(TValue value, double priority);
    (TValue Value, double Priority)? PopMin();
}

public static class PriorityQueueExtensions
{
    public static bool AddOrUpdate<TValue>(this IPriorityQueue<TValue> queue, TValue value, double priority)
    {
        var (hasValue, oldPriority) = queue.Peek(value);
        if (!hasValue)
        {
            queue.Add(value, priority);
            return true;
        }

        if (priority < oldPriority)
        {
            queue.Update(value, priority);
            return true;
        }

        return false;
    }
}