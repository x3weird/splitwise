using Splitwise.DomainModel.Models.ApplicationClasses;
using Splitwise.Repository.DataRepository;
using System;
using System.Collections.Generic;
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
    }
}
