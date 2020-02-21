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
        Task AddNotificationUser(ExpenseNotification expenseNotification);
        Task AddConnectedUser(NotificationHub notificationHub);
        Task RemoveConnectedUser(string userId);
        Task<List<NotificationHub>> GetConnectedUser();
        Task RemoveNotificationUser(string userId);
        Task<List<ExpenseNotification>> GetNotificationUser();
    }
}
