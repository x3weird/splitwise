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
        Task<int> AddGroupMembers(GroupAdd groupAdd, string email, Group group);
        Task<Group> AddGroup(GroupAdd groupAdd, string email);
        Task<List<ExpenseDetail>> GetGroupExpenseList(string groupId, string email);
        Task RemoveGroup(string groupId);
    }

}