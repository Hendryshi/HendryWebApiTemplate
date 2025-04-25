using Common.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Persistence.Interceptors
{
    public class EntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ILogger<EntitySaveChangesInterceptor> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EntitySaveChangesInterceptor(
            IServiceProvider serviceProvider,
            ILogger<EntitySaveChangesInterceptor> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext context)
        {
            if(context == null) throw new ArgumentNullException(nameof(context));

            foreach(var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
            {
                var datetimeNow = DateTime.Now;

                if(entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated(datetimeNow);
                }

                if(entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    entry.Entity.SetModified(datetimeNow);
                }
            }
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
                r.TargetEntry != null &&
                r.TargetEntry.Metadata.IsOwned() &&
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }

}
