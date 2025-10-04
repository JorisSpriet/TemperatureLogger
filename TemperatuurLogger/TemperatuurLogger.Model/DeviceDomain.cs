using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    public class DeviceDomain : DomainWrapper
    {
        public DeviceDomain(Domain domain):base(domain) { }       
    }
}