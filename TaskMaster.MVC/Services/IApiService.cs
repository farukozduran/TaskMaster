namespace TaskMaster.MVC.Services
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<TResponse> PostAsync<TResponse, TRequest>(string endpoint, TRequest data);
        Task<bool> PutAsync<TRequest>(string endpoint, TRequest data);
        Task<bool> DeleteAsync(string endpoint);
    }
}
