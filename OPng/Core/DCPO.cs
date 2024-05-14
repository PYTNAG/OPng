using System.Collections;

namespace OPng.Core;

public sealed class DCPO<T> : IEnumerable<T>
    where T : notnull
{
    public T Supremum { get; private set; }
    private readonly Dictionary<T, List<T>> _nodes;

    public DCPO(T supremum)
    {
        Supremum = supremum;
        _nodes = new(){
            [Supremum] = []
        };
    }

    public void AddBefore(T @base, T prev)
    {
        if (!_nodes.TryGetValue(@base, out List<T>? previousNodes))
        {
            previousNodes = [];
            _nodes.Add(@base, previousNodes);
        }

        previousNodes.Add(prev);
    }

    public IEnumerator<T> GetEnumerator() => new DCPOEnumerator<T>(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<T> this[T @base] => _nodes[@base];
}