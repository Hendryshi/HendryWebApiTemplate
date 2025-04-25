using Common.Infrastructure.Persistence;
using Common.Infrastructure.Persistence.Interceptors;
using HendryTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HendryTemplate.Infrastructure.Persistence
{
    public class AppDbContext : BaseDbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            EntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
            IConfiguration configuration)
            : base(options, auditableEntitySaveChangesInterceptor)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(m =>
            {
                m.HasKey(x => x.Id);
                m.Property(x => x.UserName).IsRequired();
                m.HasIndex(x => x.UserName).IsUnique();
                m.Property(x => x.Password).IsRequired();
            });
        }
    }

}
