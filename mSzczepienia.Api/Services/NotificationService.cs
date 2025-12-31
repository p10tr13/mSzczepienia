using FirebaseAdmin.Messaging;
using mSzczepienia.Api.Services.Interfaces;

namespace mSzczepienia.Api.Services
{
    public class NotificationService: INotificationService
    {
        public async Task SendNotificationAsync(string deviceToken, string title, string body)
        {
            var message = new Message()
            {
                Token = deviceToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}