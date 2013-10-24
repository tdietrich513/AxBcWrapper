using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using AutoMapper;

using AxBcWrapper.Session;

using Microsoft.Dynamics.BusinessConnectorNet;
using System.Reflection;

namespace AxBcWrapper.Mapping
{
    public abstract class ClassMapping
    {
        internal abstract void ConfigureAutoMapper();

        internal abstract AxaptaRecord GetRecordObject(AxSession session);

        public abstract Expression MappingForField(string fieldName);
        public abstract String FieldForMapping(Expression mapping);

        public abstract Type MappedType();
    }


    public abstract class ClassMapping<T> : ClassMapping
    {
        private string _tableName;
        private Dictionary<string, Expression<Func<T, object>>> _fieldMappings;
        private Dictionary<MemberInfo, string> _fieldNames;
        private Expression<Func<T, string>> _captionMap;
        private Expression<Func<T, string>> _companyMap;

        public void TableName(string tableName)
        {
            _tableName = tableName;
        }

        private MemberExpression GetMemberExpression(Expression body)
        {
            var candidates = new Queue<Expression>();
            candidates.Enqueue(body);
            while (candidates.Count > 0)
            {
                var expr = candidates.Dequeue();
                if (expr is MemberExpression)
                {
                    return ((MemberExpression)expr);
                }
                else if (expr is UnaryExpression)
                {
                    candidates.Enqueue(((UnaryExpression)expr).Operand);
                }
                else if (expr is BinaryExpression)
                {
                    var binary = expr as BinaryExpression;
                    candidates.Enqueue(binary.Left);
                    candidates.Enqueue(binary.Right);
                }
                else if (expr is MethodCallExpression)
                {
                    var method = expr as MethodCallExpression;
                    foreach (var argument in method.Arguments)
                    {
                        candidates.Enqueue(argument);
                    }
                }
                else if (expr is LambdaExpression)
                {
                    candidates.Enqueue(((LambdaExpression)expr).Body);
                }
            }

            return null;
        }

        public void Field(string fieldName, Expression<Func<T, object>> member)
        {
            if (_fieldMappings == null) _fieldMappings = new Dictionary<string, Expression<Func<T, object>>>();
            if (_fieldNames == null) _fieldNames = new Dictionary<MemberInfo, string>();

            _fieldMappings.Add(fieldName, member);
            _fieldNames.Add(GetMemberExpression(member).Member, fieldName);
        }

        public override string FieldForMapping(Expression mapping)
        {
            if (_fieldNames == null) return null;
            var x = mapping as MemberExpression;
            if (x == null) return null;
            if (!_fieldNames.ContainsKey(x.Member)) return null;

            return _fieldNames[x.Member];
        }

        public override Expression MappingForField(string fieldName)
        {
            if (_fieldMappings == null) return null;
            return _fieldMappings[fieldName];
        }

        public override Type MappedType()
        {
            return typeof(T);
        }

        public void Caption(Expression<Func<T, string>> member)
        {
            _captionMap = member;
        }
        
        public void Company(Expression<Func<T, string>> member)
        {
            _companyMap = member;
        }

        internal override AxaptaRecord GetRecordObject(AxSession session)
        {
            return session.Session.CreateAxaptaRecord(_tableName);
        }

        internal override void ConfigureAutoMapper()
        {
            var mapFrom = Mapper.CreateMap<AxaptaRecord, T>();

            if (_fieldMappings != null)
                foreach (var kvp in _fieldMappings)                
                    mapFrom.ForMember(kvp.Value, o => o.MapFrom(ar => ar.get_Field(kvp.Key)));                
                            

            var mapTo = Mapper.CreateMap<T, AxaptaRecord>();

            // Map the caption property if a mapping exists.
            if (_captionMap != null) mapTo.ForMember(ax => ax.Caption, o => o.MapFrom(_captionMap));
            else mapTo.ForMember(ax => ax.Caption, o => o.Ignore());
            
            // Map the Company property if a mapping exists.
            if (_companyMap != null) mapTo.ForMember(ax => ax.Company, o => o.MapFrom(_companyMap));
            else mapTo.ForMember(ax => ax.Company, o => o.Ignore());            

            // We aren't doing anything with Found or Ignore, yet. 
            mapTo.ForMember(ax => ax.TooltipRecord, o => o.Ignore());
            mapTo.ForMember(ax => ax.Found, o => o.Ignore());

            if (_fieldMappings != null)
                mapTo.AfterMap((t, ax) => {
                    foreach (var kvp in _fieldMappings)
                        ax.set_Field(kvp.Key, kvp.Value.Compile().Invoke(t));
                });
        }
    }
}
