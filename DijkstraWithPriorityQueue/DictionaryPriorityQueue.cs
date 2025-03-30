namespace DijkstraWithPriorityQueue;

public class DictionaryPriorityQueue<TValue> : IPriorityQueue<TValue> where TValue : notnull
{
    private readonly Dictionary<TValue, double> items = new();

    public (bool Success, double Priority) Peek(TValue value)
    {
        var success = items.TryGetValue(value, out var priority);
        return (success, priority);
    }

    public void Add(TValue value, double priority) => items.Add(value, priority);
    public void Remove(TValue value) => items.Remove(value);
    public void Update(TValue value, double newValue) => items[value] = newValue;

    public (TValue Value, double Priority)? PopMin()
    {
        if (items.Count == 0) return null;
        var min = items.Min(z => z.Value);
        var value = items.FirstOrDefault(z => z.Value == min).Key;
        items.Remove(value);
        return (value, min);
    }
}