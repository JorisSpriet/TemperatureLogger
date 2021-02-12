using System;
using Xtensive.Orm;

public static class DomainBuilder
{
    static Domain BuildDomain()
    {
        var dc = new DomainConfiguration();
        Domain d = Domain.Build(dc);
    }

    static void UnloadDomain(Domain domain)
    {
        domain.Dispose();
    }
}