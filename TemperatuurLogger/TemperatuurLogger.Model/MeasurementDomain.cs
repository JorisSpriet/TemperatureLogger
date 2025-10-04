using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    public class MeasurementDomain : DomainWrapper
    {      
        public string Year { get; private set; }

        public MeasurementDomain(string year, Domain domain) : base(domain)
        {
            Year = year;            
        }
    }
}