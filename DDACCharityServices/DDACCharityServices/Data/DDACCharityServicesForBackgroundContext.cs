using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DDACCharityServices.Models;

namespace DDACCharityServices.Data
{
    public class DDACCharityServicesForBackgroundContext : DbContext
    {
        public DDACCharityServicesForBackgroundContext (DbContextOptions<DDACCharityServicesForBackgroundContext> options)
            : base(options)
        {
        }

        public DbSet<DDACCharityServices.Models.Background> Background { get; set; }

        public DbSet<DDACCharityServices.Models.Donation> Donation { get; set; }
    }
}
