using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AxBcWrapper.Session;

namespace AxBcWrapper.Query
{
    public class AxQuery<T> : IOrderedQueryable<T>
    {


        private readonly Expression _expression;
        private readonly QueryProvider _provider;

        public AxQuery(QueryProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public AxQuery(QueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            _provider = provider;
            _expression = expression;
        }

        Type IQueryable.ElementType
        {
            get
            {
                return typeof(T);
            }
        }
        Expression IQueryable.Expression
        {
            get
            {
                return _expression;
            }
        }

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return _provider;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_provider.Execute(_expression)).GetEnumerator();
        }

        public override string ToString()
        {
            return _provider.GetQueryText(_expression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();
        }

        //public class AxQuery<T> : IQueryable<T>
        //{
        //    public IEnumerator<T> GetEnumerator()
        //    {
        //        return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        //    }

        //    IEnumerator IEnumerable.GetEnumerator()
        //    {
        //        return GetEnumerator();
        //    }

        //    public AxQuery(AxSession session)
        //    {
        //        Provider = new AxQueryProvider(session);
        //        Expression = Expression.Constant(this);
        //    }

        //    public AxQuery(AxQueryProvider provider)
        //    {
        //        if (provider == null)
        //        {
        //            throw new ArgumentNullException("provider");
        //        }

        //        Provider = provider;
        //        Expression = Expression.Constant(this);
        //    }

        //    public AxQuery(AxQueryProvider provider, Expression expression) : this(provider)
        //    {            
        //        if (expression == null)
        //        {
        //            throw new ArgumentNullException("expression");
        //        }            

        //        Expression = expression;
        //    }

        //    public Expression Expression { get; private set; }
        //    public Type ElementType { get { return typeof(T); } }
        //    public IQueryProvider Provider { get; private set; }
        //}
    }
}