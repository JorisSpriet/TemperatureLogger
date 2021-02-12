using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
   
    /// <summary>
    /// ID servers as key
    /// </summary>
    public interface IBaseObject : IEntity
    {        
        [Field, Key]
        long ID { get; }
    }
}