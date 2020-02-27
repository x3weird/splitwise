
using Splitwise.DomainModel.Models;
using Splitwise.DomainModel.Models.ApplicationClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.NotificationRepository
{
    public interface INotificationRepository
    {
        Task AddNotificationUser(Notification notification);
        Task AddConnectedUser(NotificationHub notificationHub);
        Task RemoveConnectedUser(string userId);
        Task<List<NotificationHub>> GetConnectedUser();
        Task RemoveNotificationUser(string userId);
        Task<List<Notification>> GetNotificationUser();
    }
}
