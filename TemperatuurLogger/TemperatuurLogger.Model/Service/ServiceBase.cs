using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    public abstract class ServiceBase
    {
        protected void ExecuteTransactionally(Domain domain, Action action)
        {
            using (var session = domain.OpenSession())
            {
                if (!session.IsActive)
                    session.Activate();
                using (var trn = session.OpenTransaction())
                {

                    action();

                    trn.Complete();
                }
            }
        }
        
    }
}
