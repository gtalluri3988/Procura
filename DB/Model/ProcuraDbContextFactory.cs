using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Model
{
    public class ProcuraDbContextFactory
        : IDesignTimeDbContextFactory<ProcuraDbContext>
    {
        public ProcuraDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ProcuraDbContext>();

            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("ProcuraConnection"));

            return new ProcuraDbContext(optionsBuilder.Options);
        }
    }
}
