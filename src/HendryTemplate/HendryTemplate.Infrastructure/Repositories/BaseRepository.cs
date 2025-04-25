using Common.Application.Interfaces;
using Common.Application.Services.Helpers;
using Common.Domain.Common;
using Common.Domain.Common.Events;
using FluentResults;
using HendryTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq.Expressions;

namespace HendryTemplate.Infrastructure.Repositories
{
    public class BaseRepository : IAsyncRepository
    {
        private readonly ILogger<BaseRepository> _logger;
        protected readonly AppDbContext _dbContext;

        public BaseRepository(AppDbContext dbContext, ILogger<BaseRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger;
        }

        public IQueryable<T> GetDefaultPredicateQuery<T>(IQueryable<T> mQuery = null) where T : class, new()
        {
            IQueryable<T> query;

            if(mQuery == null)
            {
                query = _dbContext.Set<T>().AsQueryable();
            }
            else
            {
                query = mQuery;
            }

            query = IncludeNavigation(query);

            return query.AsNoTracking();
        }

        public IQueryable<T> GetDefaultDbSetQuery<T>() where T : class, new()
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();

            return query.AsNoTracking();
        }

        public async Task<Result<List<TResult>>> GetAsync<T, TJoin, TResult>(
            Expression<Func<T, bool>> filter = null,
            Expression<Func<TJoin, bool>> joinFilter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            Expression<Func<T, object>> joinKey = null,
            Expression<Func<TJoin, object>> joinKeyJoin = null,
            Expression<Func<T, TJoin, TResult>> selectResult = null)
            where T : class, new()
            where TJoin : class, new()
            where TResult : class
        {
            try
            {
                IQueryable<T> query = this.GetDefaultPredicateQuery<T>();


                if(filter != null)
                {
                    query = query.Where(filter);
                }


                includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                 .ToList()
                                 .ForEach(include => query = query.Include(include));


                IQueryable<TResult> resultQuery = null;
                if(joinKey != null && joinKeyJoin != null && selectResult != null)
                {
                    var joinQuery = _dbContext.Set<TJoin>();
                    resultQuery = query.Join(
                        joinQuery.Where(joinFilter ?? (x => true)),
                        joinKey,
                        joinKeyJoin,
                        selectResult
                    );
                }
                else
                {
                    // If no join, simply T to TResult
                    resultQuery = query.Select(x => (TResult)(object)x);
                }

                if(orderBy != null)
                {
                    resultQuery = orderBy(query).AsNoTracking().Select(x => (TResult)(object)x);
                }
                var list = await resultQuery.AsNoTracking().ToListAsync();
                return Result.Ok(list);
            }
            catch(Exception e)
            {
                return ResultHelper.MapToResult(e);
            }
        }

        public async Task<Result<List<T>>> GetAsync<T>(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
            where T : class, new()
        {
            try
            {
                IQueryable<T> query = this.GetDefaultPredicateQuery<T>();

                if(filter != null)
                {
                    query = query.Where(filter);
                }

                includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => query = query.Include(x));

                var list = new List<T>();
                if(orderBy != null)
                {
                    list = await orderBy(query).AsNoTracking().ToListAsync();
                }
                else
                {
                    list = await query.AsNoTracking().ToListAsync();
                }

                return Result.Ok(list);
            }
            catch(Exception e)
            {
                return ResultHelper.MapToResult(e);
            }
        }

        public async Task<Result<List<T>>> GetAsync<T>(IQueryable<T> query) where T : class, new()
        {
            try
            {
                var list = await query.AsNoTracking().ToListAsync();

                return Result.Ok(list);
            }
            catch(Exception e)
            {
                return ResultHelper.MapToResult(e);
            }
        }

        public async Task<Result> DeleteAsync<T>(T entity) where T : class, new()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(entity);
                var result = _dbContext.Remove(entity);
                if(entity is BaseEntity baseEntity)
                {
                    baseEntity.AddDomainEvent(baseEntity.NewEvent(EventAction.Delete));
                }
                return Result.Ok();
            }
            catch(Exception e)
            {
                return ResultHelper.MapToResult(e);
            }
        }

        public async Task<Result<T>> AddAsync<T>(T entity) where T : class, new()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(entity);
                var result = await _dbContext.AddAsync(entity);
                if(entity is BaseEntity baseEntity)
                {
                    baseEntity.AddDomainEvent(baseEntity.NewEvent(EventAction.Create));
                }
                return Result.Ok(result.Entity);
            }
            catch(Exception e)
            {
                return ResultHelper.MapToResult(e);
            }
        }

        public async Task<Result<T>> UpdateAsync<T>(T entity) where T : class, new()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(entity);
                var result = _dbContext.Update(entity);
                if(entity is BaseEntity baseEntity)
                {
                    baseEntity.AddDomainEvent(baseEntity.NewEvent(EventAction.Update));
                }
                return Result.Ok(result.Entity);
            }
            catch(Exception e)
            {
                return ResultHelper.MapToResult(e);
            }
        }

        public async Task<Result> CommitOperationsAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch(Exception e)
            {
                return ResultHelper.MapToResult(e);
            }
        }


        #region private function
        private static IEnumerable<string> GetNavigations(IEntityType entityType, string path = null)
        {
            var navigations = entityType.GetNavigations();
            var navNames = navigations.Select(n => path == null ? n.Name : path + "." + n.Name).ToList();

            foreach(var nav in navigations)
            {
                var tempPath = path == null ? nav.Name : path + "." + nav.Name;
                navNames.AddRange(GetNavigations(nav.TargetEntityType, tempPath));
            }

            return navNames;
        }
        private IQueryable<T> IncludeNavigation<T>(IQueryable<T> mQuery) where T : class, new()
        {
            if(mQuery == null) throw new ArgumentNullException("The query cannot be null");

            IQueryable<T> query = mQuery;

            var entityType = _dbContext.Model.FindEntityType(typeof(T));
            var navigations = GetNavigations(entityType);

            foreach(var navName in navigations)
            {
                query = query.Include(navName);
            }

            return query;
        }
        #endregion
    }
}
