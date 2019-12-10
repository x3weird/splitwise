using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.ExpenseRepository
{
    public interface IExpenseRepository
    {
        Task<List<ExpenseDetail>> GetExpenseList(string email);
        Task<int> DeleteExpense(string expenseId, string currentUserId);
        Task AddExpense(AddExpense addExpense);
        Task<List<UserExpense>> Dashboard(string email);
        Task SettleUp(SettleUp settleUp, string email);
        Task UnDeleteExpense(string expenseId, string currentUserId);
    }
}
