using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.GroupRepository
{
    public interface IGroupRepository
    {
        List<UserNameWithId> GetGroupList();
        int AddGroup(GroupAdd groupAdd);
        int EditGroup(string groupId, GroupAdd groupAdd);
        GroupDetails GetGroupDetails(string groupId);
        List<UserExpense> GroupUserExpense(string groupId, List<string> users);
        Task<List<ExpenseDetail>> GetGroupExpenseList(string groupId, string email);
    }

}