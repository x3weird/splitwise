using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.FriendRepository
{
    public interface IFriendRepository
    {
        Task<string> InviteFriend(InviteFriend inviteFriend, string email);
        Task<List<UserNameWithId>> GetFriendList(string userId);
        Task<List<ExpenseDetail>> GetFriendExpenseList(string friendId, string email);
        Task<UserExpense> UserExpense(string userId);
        Task RemoveFriend(string friendId, string userId);
        Task RegisterNewFriends(InviteFriend inviteFriend, string currentUserId);

    }
}
