using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
using Splitwise.DomainModel.Models.ApplicationClasses;
using Splitwise.Repository.DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.NotificationRepository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDataRepository _dal;

        public NotificationRepository(IDataRepository dal)
        {
            _dal = dal;
        }

        public async Task AddNotificationUser(string userId, string expenseId)
        {
            ExpenseNotification expenseNotification = new ExpenseNotification()
            {
                ExpenseId = expenseId,
                UserId = userId
            };

            await _dal.AddAsync<ExpenseNotification>(expenseNotification);
        }

        public async Task AddConnectedUser(NotificationHub notificationHub)
        {
            await _dal.AddAsync<NotificationHub>(notificationHub);
        }

        public async Task RemoveConnectedUser(string userId)
        {
            List<NotificationHub> notificationHubR =  await _dal.Where<NotificationHub>(n => n.UserId.Equals(userId) ).ToListAsync();
            _dal.RemoveRange<NotificationHub>(notificationHubR);
        }

        public async Task<List<NotificationHub>> GetConnectedUser()
        {
            return await _dal.Get<NotificationHub>();
        }
    }
}
