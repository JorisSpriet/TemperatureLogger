using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
   
    /// <summary>
    /// Base object.
    /// </summary>
    public abstract class BaseObject : Entity
    {       
        [Field, Key]
        public long ID { get; private set;}
    }
}