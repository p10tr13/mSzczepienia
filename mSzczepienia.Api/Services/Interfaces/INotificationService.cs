namespace mSzczepienia.Api.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string deviceToken, string title, string body);
    }
}