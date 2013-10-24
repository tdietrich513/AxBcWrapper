using System.Linq.Expressions;

namespace AxBcWrapper.Query
{
    internal class ColumnProjection
    {
        internal string Columns;
        internal Expression Selector;
    }
}