using FluentResults;
using System.Linq.Expressions;

namespace Common.Application.Interfaces
{
    public interface IAsyncRepository
    {
        Task<Result<T>> AddAsync<T>(T entity) where T : class, new();
        Task<Result> CommitOperationsAsync();
        Task<Result> DeleteAsync<T>(T entity) where T : class, new();
        Task<Result<List<TResult>>> GetAsync<T, TJoin, TResult>(Expression<Func<T, bool>> filter = null, Expression<Func<TJoin, bool>> joinFilter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", Expression<Func<T, object>> joinKey = null, Expression<Func<TJoin, object>> joinKeyJoin = null, Expression<Func<T, TJoin, TResult>> selectResult = null)
            where T : class, new()
            where TJoin : class, new()
            where TResult : class;
        Task<Result<List<T>>> GetAsync<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "") where T : class, new();
        Task<Result<List<T>>> GetAsync<T>(IQueryable<T> query) where T : class, new();
        IQueryable<T> GetDefaultDbSetQuery<T>() where T : class, new();
        IQueryable<T> GetDefaultPredicateQuery<T>(IQueryable<T> mQuery = null) where T : class, new();
        Task<Result<T>> UpdateAsync<T>(T entity) where T : class, new();
    }
}
