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
        Task<Expense> DeleteExpense(string expenseId, string currentUserId);
        Task<Expense> AddExpense(AddExpense addExpense);
        Task<Activity> AddExpenseActivity(AddExpense addExpense, Expense expense);
        Task AddExpenseInLedger(AddExpense addExpense, Expense expense, Activity activity);
        Task<Expense> AddSettleUpExpense(SettleUp settleUp, string email);
        Task<List<UserExpense>> Dashboard(string email);
        Task SettleUp(SettleUp settleUp, string email, Expense expense);
        Task<Expense> UnDeleteExpense(string expenseId, string currentUserId);
        Task<List<string>> GetUniqueLedgerUsers(string expenseId);
    }
}
