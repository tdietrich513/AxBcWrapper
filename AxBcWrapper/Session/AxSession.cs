using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;

using AutoMapper;

using AxBcWrapper.Mapping;
using AxBcWrapper.Query;

using Microsoft.Dynamics.BusinessConnectorNet;

namespace AxBcWrapper.Session
{
    public class AxSession : IDisposable
    {
        private readonly Axapta _session;

        internal Axapta Session
        {
            get
            {
                return _session;
            }
        }

        internal Dictionary<Type, ClassMapping> Mappings { get; set; }

        internal AxaptaRecord GetRecordObject<T>()
        {
            Type qType;

            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T).IsGenericType) 
                qType = typeof(T).GetGenericArguments()[0];
            else
                qType = typeof(T);

            var map = Mappings[qType];
            return map.GetRecordObject(this);
        }

        public IEnumerable Query(string query, Type elementType)
        {
            var map = Mappings[elementType];
            var axRec = map.GetRecordObject(this);

            axRec.ExecuteStmt(query);
            var rlist = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            if (!axRec.Found) return rlist;
            do
            {
                rlist.Add(Mapper.Map(axRec, Activator.CreateInstance(elementType)));
            }
            while (axRec.Next());

            return rlist;
        }

        public IEnumerable<T> Query<T>(string query)
        {
            var axRec = GetRecordObject<T>();
            axRec.ExecuteStmt(query);

            var rlist = new List<T>();

            if (!axRec.Found) return rlist;
            do
            {
                rlist.Add(Mapper.Map<T>(axRec));
            }
            while (axRec.Next());

            return rlist;
        }

        public IQueryable<T> Query<T>()
        {
            var prov = new AxQueryProvider(this);
            return new AxQuery<T>(prov);
        }

        public AxSession()
        {
            _session = new Axapta();
        }

        public void Dispose()
        {
            _session.Logoff();
        }
    }
}