using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _innerQueryProvider;

    public TestAsyncQueryProvider(IQueryProvider innerQueryProvider)
    {
        _innerQueryProvider = innerQueryProvider;
    }

    public IQueryable CreateQuery(Expression expression)
        => new TestAsyncEnumerable<TEntity>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncEnumerable<TElement>(expression);

    public object Execute(Expression expression)
        => _innerQueryProvider.Execute(expression);

    public TResult Execute<TResult>(Expression expression)
        => _innerQueryProvider.Execute<TResult>(expression);

    public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        => new TestAsyncEnumerable<TResult>(expression);

    TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
