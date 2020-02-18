using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.NotificationRepository
{
    public interface INotificationRepository
    {
        Task AddNotificationUser(string userId, string expenseId);
    }
}
