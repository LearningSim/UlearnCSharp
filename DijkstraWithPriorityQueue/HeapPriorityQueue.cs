namespace DijkstraWithPriorityQueue;

public class HeapPriorityQueue<TValue> : IPriorityQueue<TValue> where TValue : notnull
{
    private readonly Dictionary<TValue, int> indexByVal = new();
    private readonly List<(TValue Value, double Priority)> items = [];

    public (bool Success, double Priority) Peek(TValue value)
    {
        var success = indexByVal.TryGetValue(value, out var i);
        return (success, success ? items[i].Priority : double.PositiveInfinity);
    }

    public void Add(TValue value, double priority)
    {
        indexByVal[value] = items.Count;
        items.Add((value, priority));
        HeapifyUp(items.Count - 1);
    }

    public void Remove(TValue value)
    {
        if (items.Count == 0) return;

        var index = indexByVal[value];
        indexByVal[items[^1].Value] = index;
        indexByVal.Remove(value);

        items[index] = items[^1];
        items.RemoveAt(items.Count - 1);
        if (items.Count == 0) return;

        HeapifyDown(index);
    }

    public void Update(TValue value, double priority)
    {
        var index = indexByVal[value];
        var oldPriority = items[index].Priority;
        items[index] = items[index] with { Priority = priority };
        if (priority > oldPriority)
        {
            HeapifyDown(index);
        }
        else
        {
            HeapifyUp(index);
        }
    }

    public (TValue Value, double Priority)? PopMin()
    {
        if (items.Count == 0) return null;
        var min = items[0];
        Remove(items[0].Value);
        return min;
    }

    private void HeapifyUp(int itemIndex)
    {
        while (ParentIndex(itemIndex) >= 0 && items[itemIndex].Priority < Parent(itemIndex).Priority)
        {
            var parentIndex = ParentIndex(itemIndex);
            Swap(itemIndex, parentIndex);
            itemIndex = parentIndex;
        }
    }

    private void HeapifyDown(int itemIndex)
    {
        while (true)
        {
            var minChildIndex = LeftChildIndex(itemIndex);
            if (minChildIndex >= items.Count) return;

            if (RightChildIndex(itemIndex) < items.Count &&
                RightChild(itemIndex).Priority < LeftChild(itemIndex).Priority)
            {
                minChildIndex = RightChildIndex(itemIndex);
            }

            if (items[minChildIndex].Priority >= items[itemIndex].Priority) return;

            Swap(itemIndex, minChildIndex);
            itemIndex = minChildIndex;
        }
    }

    private (TValue Value, double Priority) LeftChild(int index) => items[2 * index + 1];
    private int LeftChildIndex(int index) => 2 * index + 1;
    private (TValue Value, double Priority) RightChild(int index) => items[2 * index + 2];
    private int RightChildIndex(int index) => 2 * index + 2;

    private (TValue Value, double Priority) Parent(int index) => items[(index - 1) / 2];
    private int ParentIndex(int index) => (index - 1) / 2;

    private void Swap(int indexA, int indexB)
    {
        (items[indexA], items[indexB]) = (items[indexB], items[indexA]);
        indexByVal[items[indexA].Value] = indexA;
        indexByVal[items[indexB].Value] = indexB;
    }
}