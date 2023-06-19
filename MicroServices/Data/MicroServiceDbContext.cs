using MicroServices.Model;
using Microsoft.EntityFrameworkCore;

namespace MicroServices.Data;

public class MicroServiceDbContext : DbContext
{
    public MicroServiceDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Person>? Persons { get; set; }
}