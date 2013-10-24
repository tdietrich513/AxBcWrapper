using System.Linq.Expressions;

using AutoMapper;

using AxBcWrapper.Mapping;
using AxBcWrapper.Session;

namespace AxBcWrapper.Query
{
    public class AxQueryProvider :  QueryProvider
    {

        private readonly AxSession _session;
       

        public AxQueryProvider(AxSession session)
        {
            _session = session;
        }                

        public override object Execute(Expression expression)
        {
            var elementType = TypeSystem.GetElementType(expression.Type);

            var mapping = _session.Mappings[elementType];

            var result = Translate(expression, mapping);            
            var query = result.CommandText;
            

            var results = _session.Query(query, elementType);
          
            return results;
        }

        public override string GetQueryText(Expression expression)
        {
            var elementType = TypeSystem.GetElementType(expression.Type);

            var mapping = _session.Mappings[elementType];

            return Translate(expression, mapping).CommandText;
        }
        
        
        private TranslateResult Translate(Expression expression, ClassMapping mapping)
        {
            expression = new Evaluator().PartialEval(expression);
            return new QueryTranslator().Translate(expression, mapping);
        }

        

        //private readonly AxSession _session;
        //public AxQueryProvider(AxSession session)
        //{
        //    _session = session;
        //}

        //public IQueryable CreateQuery(Expression expression)
        //{
        //    throw new NotImplementedException();
        //}

        //public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        //{
        //    return new AxQuery<TElement>(this, expression);
        //}

        //public object Execute(Expression expression)
        //{
        //    throw new NotImplementedException();
        //}

        //public TResult Execute<TResult>(Expression expression)
        //{
        //    var statement = Translate(expression);

        //    var axRec = _session.GetRecordObject<TResult>();            
        //    axRec.ExecuteStmt(statement);

        //    return Mapper.Map<TResult>(axRec);
        //}

        //private string Translate(Expression expression)
        //{
        //    expression = new Evaluator().PartialEval(expression);
        //    return new QueryTranslator().Translate(expression);
        //}
    }
}