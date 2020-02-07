using System;
using System.Linq;
using System.Linq.Expressions;

namespace nUnitExpressionTest.Linq
{
    public class QueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            return new Query<object>(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
