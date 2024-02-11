using System;
using TemperatuurLogger.Model;
using Xtensive.Orm;

namespace TemperatuurLogger.UI.ViewModels
{
	public abstract class DatabaseBoundViewModelBase : ViewModelBase
	{
		private static object sync = new object();

		protected static Domain domain;

		protected void ExecuteTransactionally( Action action)
		{			
			using (var session = domain.OpenSession()) {
				if (!session.IsActive)
					session.Activate();
				using (var trn = session.OpenTransaction()) {

					action();
					
					trn.Complete();
				}
			}
		}

		private void DemandDomain()
		{
			lock (sync) {
				if (domain != null)
					return;
				domain = DomainBuilder.Domain ?? DomainBuilder.BuildDomain();
			}
		}

		public DatabaseBoundViewModelBase()
		{
			DemandDomain();
		}

		
	}
}
