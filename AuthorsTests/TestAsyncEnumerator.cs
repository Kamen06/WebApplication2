using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _innerEnumerator;

    public TestAsyncEnumerator(IEnumerator<T> innerEnumerator)
        => _innerEnumerator = innerEnumerator;

    public T Current => _innerEnumerator.Current;

    public ValueTask DisposeAsync()
    {
        _innerEnumerator.Dispose();
        return default;
    }

    public ValueTask<bool> MoveNextAsync()
        => new ValueTask<bool>(_innerEnumerator.MoveNext());
}
