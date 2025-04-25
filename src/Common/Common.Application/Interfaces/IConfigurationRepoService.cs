using FluentResults;

namespace Common.Application.Interfaces
{
    public interface IConfigurationRepoService
    {
        public Task<Result<List<T>>> ListAsync<T>() where T : class, new();

        public Task<Result<T>> GetAsync<T>(string id) where T : class, new();

        public Task<Result<T>> AddAsync<T>(T oobject) where T : class, new();

        public Task<Result<T>> UpdateAsync<T>(T oobject) where T : class, new();

        public Task<Result> DeleteAsync<T>(string id) where T : class, new();

        public Task<Result> CommitOperationsAsync();
    }
}
