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

        public async Task AddNotificationUser(ExpenseNotification expenseNotification)
        {
            var userId = await _dal.Where<ApplicationUser>(u => u.Email.Equals(expenseNotification.Email)).Select(s=>s.Id).SingleOrDefaultAsync();
            expenseNotification.UserId = userId;
            await _dal.AddAsync<ExpenseNotification>(expenseNotification);
        }

        public async Task RemoveNotificationUser(string userId)
        {
            List<ExpenseNotification> expenseNotifications = await _dal.Where<ExpenseNotification>(e => e.UserId.Equals(userId)).ToListAsync();
            _dal.RemoveRange<ExpenseNotification>(expenseNotifications);
        }

        public async Task<List<ExpenseNotification>> GetNotificationUser()
        {
            return await _dal.Get<ExpenseNotification>();
        }

        public async Task AddConnectedUser(NotificationHub notificationHub)
        {
            var email = await _dal.Where<ApplicationUser>(a => a.Id.Equals(notificationHub.UserId)).Select(s=>s.Email).FirstOrDefaultAsync();
            notificationHub.Email = email;
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
