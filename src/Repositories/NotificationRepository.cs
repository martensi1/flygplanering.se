using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FlightPlanner.Service.Repositories
{
    public interface INotificationRepository
    {
        public string GetRandomNotification();
    }


    public class NotificationRepository : INotificationRepository
    {
        private readonly ILogger<NotificationRepository> _logger;
        private IReadOnlyList<string> _notifications;


        public NotificationRepository(
            ILogger<NotificationRepository> logger
            )
        {
            _logger = logger;
            CreateNotificationList();
        }


        public string GetRandomNotification()
        {
            var random = new Random();
            int index = random.Next(_notifications.Count);

            return _notifications[index];
        }


        private void CreateNotificationList()
        {
            _notifications = new List<string>() {
                "Saknar du vissa flygplatser? Gå till <a href=\"/Settings\">dina inställningar</a> för att ändra vilka du kan se!",
                "Är du medlem i Jönköpings Flygklubb? Gå till <a href=\"https://jfk.flygplanering.se\">jfk.flygplanering.se</a> för att även kunna räkna ut vikt och balans på klubbens plan!"
            };

            _logger.LogInformation("Registered {0} notifications", _notifications.Count);
        }
    }
}
