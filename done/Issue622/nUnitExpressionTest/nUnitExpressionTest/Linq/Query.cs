using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace nUnitExpressionTest.Linq
{
    public class Query<T> : IOrderedQueryable<T>
    {
        public Query(IQueryProvider provider, Expression e = null)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Expression = e ?? Expression.Constant(this);
        }

        public Expression Expression { get; }

        public Type ElementType => typeof(T);

        public IQueryProvider Provider { get; }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Provider.Execute(Expression))?.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)Provider.Execute(Expression))?.GetEnumerator();
        }
    }
}
