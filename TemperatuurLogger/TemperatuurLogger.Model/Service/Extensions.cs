using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    public static class Extensions
    {

        public static void With(this DomainWrapper dw, Action action)
        {
            var domain = dw.Domain;
            ExecuteTransactionally(domain, action);
        }

        static void ExecuteTransactionally(Domain domain, Action action)
        {
            var session = (Session.Current?.Domain == domain) ?
                 Session.Current : domain.OpenSession();            
            if(session.IsDisposed)
                session=domain.OpenSession();

            if (!session.IsActive)
                session.Activate();
            ExecuteTransactionally(session, action);
        }

        static void ExecuteTransactionally(Session session, Action action)
        {
            using (var trn = session.OpenTransaction())
            {
                action();
                trn.Complete();
            }
        }

    }
}
