using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.NotificationRepository
{
    public interface INotificationRepository
    {
        Task AddNotificationUser(string userId, string expenseId);
        Task AddConnectedUser(NotificationHub notificationHub);
        Task RemoveConnectedUser(string userId);
        Task<List<NotificationHub>> GetConnectedUser();
    }
}
