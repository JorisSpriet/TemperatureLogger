using Xtensive.Orm;

namespace TemperatuurLogger.Model
{
    public abstract class DomainWrapper
    {
        private Session session;

        public Domain Domain { get; private set; }

        public Session Session
        {
            get
            {
                if (session == null)
                    session = this.Domain.OpenSession();
                if (!session.IsActive)
                    session.Activate();
                return session;
            }
        }
        public DomainWrapper(Domain domain)
        {
            Domain = domain;
        }
    }
}