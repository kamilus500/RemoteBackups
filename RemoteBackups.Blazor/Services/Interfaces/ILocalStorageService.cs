namespace RemoteBackups.Blazor.Services.Interfaces
{
    public interface ILocalStorageService
    {
        Task<string?> GetItemAsync(string key);
        Task SetItemAsync(string key, string value);
        Task RemoveItemAsync(string key);
    }
}
