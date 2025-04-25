using Common.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Persistence
{
    public abstract class BaseDbContext : DbContext
    {
        private readonly EntitySaveChangesInterceptor _baseEntityInterceptor;

        public BaseDbContext(DbContextOptions options,
            EntitySaveChangesInterceptor baseEntityInterceptor) : base(options)
        {
            _baseEntityInterceptor = baseEntityInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_baseEntityInterceptor);
            base.OnConfiguring(optionsBuilder);
        }
        public override int SaveChanges()
        {
            var r = base.SaveChanges();
            this.ChangeTracker.Clear();
            return r;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var r = await base.SaveChangesAsync(cancellationToken);
            this.ChangeTracker.Clear();
            return r;
        }

    }
}
