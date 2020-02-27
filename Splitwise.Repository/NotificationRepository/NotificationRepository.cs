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

        public async Task AddNotificationUser(Notification notification)
        {
            if(notification.Email != null && notification.UserId == null)
            {
                var userId = await _dal.Where<ApplicationUser>(u => u.Email.Equals(notification.Email)).Select(s => s.Id).SingleOrDefaultAsync();
                notification.UserId = userId;
            }

            if (notification.UserId != null && notification.Email == null)
            {
                var email = await _dal.Where<ApplicationUser>(u => u.Id.Equals(notification.UserId)).Select(s => s.Email).SingleOrDefaultAsync();
                notification.Email = email;
            }

            await _dal.AddAsync<Notification>(notification);
        }

        public async Task RemoveNotificationUser(string userId)
        {
            List<Notification> notifications = await _dal.Where<Notification>(e => e.UserId.Equals(userId)).ToListAsync();
            _dal.RemoveRange<Notification>(notifications);
        }

        public async Task<List<Notification>> GetNotificationUser()
        {
            return await _dal.Get<Notification>();
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
