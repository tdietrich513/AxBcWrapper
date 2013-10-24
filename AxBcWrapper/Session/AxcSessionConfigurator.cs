using System;
using System.IO;

namespace AxBcWrapper.Session
{
    public class AxcSessionConfigurator : IAxSessionConfigurator, IDisposable
    {
        private readonly FileInfo _axcFileLocation;

        public AxcSessionConfigurator(string configurationPath)
        {
            _axcFileLocation = new FileInfo(configurationPath);
            if (!_axcFileLocation.Exists) throw new FileNotFoundException("Unable to find specified configuration.", configurationPath);
        }

        public void ConfigureSession(AxSession session)
        {
            session.Session.Logon(null, null, null, _axcFileLocation.FullName);            
        }

        public void Dispose()
        {

        }
    }
}