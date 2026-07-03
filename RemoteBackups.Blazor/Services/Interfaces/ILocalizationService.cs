namespace RemoteBackups.Blazor.Services.Interfaces
{
    public interface ILocalizationService
    {
        Task InitializeAsync();
        string GetString(string key);
        string GetCurrentCulture();
    }
}
