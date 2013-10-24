using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AxBcWrapper.Query
{
    public abstract class QueryProvider : IQueryProvider
    {    

        public abstract object Execute(Expression expression);

        public abstract string GetQueryText(Expression expression);

        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new AxQuery<S>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return
                    (IQueryable)
                    Activator.CreateInstance(
                        typeof(AxQuery<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression);
        }        
    }
}