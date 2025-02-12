using System.Collections;

namespace OPng.Core;

public sealed class DCPOEnumerator<T> : IEnumerator<T> where T : notnull
{
    private readonly DCPO<T> _dcpo;

    private readonly HashSet<T> _checkedNodes = [];
    private readonly Stack<T> _nodesToCheck = [];

    private T? _current;
    public T Current => _current!;
    object IEnumerator.Current => _current!;

    public DCPOEnumerator(DCPO<T> dcpo)
    {
        _dcpo = dcpo;
        Reset();
    }

    public void Dispose()
    {
        
    }

    public bool MoveNext()
    {
        if (!_nodesToCheck.TryPeek(out T? node))
        {
            return false;
        }

        _current = IterateNext(node);
        _checkedNodes.Add(_current);
        return true;
    }

    private T IterateNext(T node)
    {
        bool nodesAdded = false;
        
        foreach (T previousNode in _dcpo[node])
        {
            if (!_checkedNodes.Contains(previousNode))
            {
                nodesAdded = true;
                _nodesToCheck.Push(previousNode);
            }
        }

        return nodesAdded ? IterateNext(_nodesToCheck.Peek()) : _nodesToCheck.Pop();
    }

    public void Reset()
    {
        _nodesToCheck.Clear();
        _nodesToCheck.Push(_dcpo.Supremum);

        _checkedNodes.Clear();
    }
}