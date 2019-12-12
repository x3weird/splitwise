using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.GroupRepository
{
    public interface IGroupRepository
    {
        Task<List<UserNameWithId>> GetGroupList();
        Task<int> AddGroup(GroupAdd groupAdd, string email);
        Task<GroupDetails> GetGroupDetails(string groupId);
        Task<List<UserExpense>> GroupUserExpense(string groupId, List<string> users);
        Task<List<ExpenseDetail>> GetGroupExpenseList(string groupId, string email);
        Task RemoveGroup(string groupId);
    }

}