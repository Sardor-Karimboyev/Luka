using Microsoft.EntityFrameworkCore;

namespace Luka.Persistence.Postgre;

public interface IApplicationDbContext
{
    DbContext Context { get; }
}