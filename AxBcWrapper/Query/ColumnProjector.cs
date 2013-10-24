using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AxBcWrapper.Query
{
    internal class ColumnProjector : ExpressionVisitor
    {
        #region Constants and Fields

        private static MethodInfo miGetValue;
        private int iColumn;
        private ParameterExpression row;
        private StringBuilder sb;

        #endregion

        #region Constructors and Destructors

        internal ColumnProjector()
        {
            if (miGetValue == null)
            {
                miGetValue = typeof(ProjectionRow).GetMethod("GetValue");
            }
        }

        #endregion

        #region Methods

        internal ColumnProjection ProjectColumns(Expression expression, ParameterExpression row)
        {
            sb = new StringBuilder();
            this.row = row;
            Expression selector = Visit(expression);
            return new ColumnProjection { Columns = sb.ToString(), Selector = selector };
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(m.Member.Name);
                return Expression.Convert(Expression.Call(row, miGetValue, Expression.Constant(iColumn++)), m.Type);
            }
            else
            {
                return base.VisitMemberAccess(m);
            }
        }

        #endregion
    }
}