using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.FriendRepository
{
    public interface IFriendRepository
    {
        string InviteFriend(InviteFriend inviteFriend, string email);
        List<UserNameWithId> GetFriendList(string userId);
        Task<List<ExpenseDetail>> GetFriendExpenseList(string friendId, string email);
        UserExpense UserExpense(string userId);
        void RemoveFriend(string friendId, string userId);
        
    }
}
