using System;
using TemperatuurLogger.Model;

namespace TemperatuurLogger.ImportLogp
{
    class Program
    {
        static void Main(string serialNumber, string logpFile, bool verbose=false, bool create=false)
        {
            var args = new Arguments(serialNumber, logpFile, create, verbose)
                .Validate();

            var i = new Importer();
            var data = i.Import(logpFile);

            var domain = DomainBuilder.BuildDomain(create);
            try
            {
                using (var session = domain.OpenSession())
                {
                    if (!session.IsActive)
                        session.Activate();
                    using (var trn = session.OpenTransaction())
                    {
                        var persister = new Persister();
                        persister.Persist(data);
                        trn.Complete();
                    }
                }
                Console.WriteLine($"Import of file {logpFile} successfull.");
            } catch(Exception e) {
                var c = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Import failed : {e.Message}");
                if(verbose)
                    Console.WriteLine(e.StackTrace);
                Console.ForegroundColor = c;
                Environment.ExitCode = 1;
            }
        }
    }
}
