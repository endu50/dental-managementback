using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace DentalDana
{
    public class DentalContext : IdentityDbContext<AppUser>
    {
        public DentalContext(DbContextOptions<DentalContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appoint> Appoints { get; set; }
        public DbSet <User>Users { get; set; }
    }
}

