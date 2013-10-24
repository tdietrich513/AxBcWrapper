using System.Linq.Expressions;

namespace AxBcWrapper.Query
{
    internal class TranslateResult
    {
        internal string CommandText;
        internal LambdaExpression Projector;
    }
}