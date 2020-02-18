using Splitwise.Repository.ActivityRepository;
using Splitwise.Repository.CommentRepository;
using Splitwise.Repository.ExpenseRepository;
using Splitwise.Repository.FriendRepository;
using Splitwise.Repository.GroupRepository;
using Splitwise.Repository.NotificationRepository;
using Splitwise.Repository.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IActivityRepository Activity { get; }
        ICommentRepository Comment { get; }
        IExpenseRepository Expense { get; }
        IFriendRepository Friend { get; }
        IUserRepository User { get; }
        IGroupRepository Group { get; }
        INotificationRepository Notification { get; }

        Task<int> Commit();
    }
}
