using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AxBcWrapper.Mapping;

namespace AxBcWrapper.Session
{
    public class AxSessionFactory : IDisposable
    {
        private readonly IAxSessionConfigurator _configurator;
        private Dictionary<Type, ClassMapping> _mappings;
        
        private static bool _mappingsCompiled = false;

        private void CompileMappings()
        {
            var type = typeof(ClassMapping);
            var types =
                AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(s => s.GetTypes())
                         .Where(p => type.IsAssignableFrom(p) && p != type);

            _mappings = new Dictionary<Type, ClassMapping>();
            foreach (var cmt in types)
            {
                var ctor = cmt.GetConstructor(new Type[] { });
                if (ctor == null) continue; // If the Class Mapping doesn't have a paramaterless constructor, we can't use it.                
                
                var cm = (ClassMapping)ctor.Invoke(new object[] { });
                cm.ConfigureAutoMapper();

                _mappings.Add(cm.MappedType(), cm);
            }

            _mappingsCompiled = true;
        }                

        public AxSessionFactory(IAxSessionConfigurator configurator)
        {
            _configurator = configurator;

            if (!_mappingsCompiled)
            {
               CompileMappings();
            }
        }

        public AxSession CreateSession()
        {
            var session = new AxSession();
            _configurator.ConfigureSession(session);
            session.Mappings = _mappings;
            return session;
        }

        public void Dispose()
        {
            var disposable = _configurator as IDisposable;
            
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
