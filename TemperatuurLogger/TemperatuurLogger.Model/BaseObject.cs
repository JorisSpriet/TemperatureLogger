using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
   
    /// <summary>
    /// Base object.
    /// </summary>
    public abstract class BaseObject : Entity
    {       
        public long ID { get; private set;}
    }
}