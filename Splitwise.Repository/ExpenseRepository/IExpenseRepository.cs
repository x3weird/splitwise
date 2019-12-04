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
        int DeleteExpense(string expenseId);
        int EditExpense(AddExpense addExpense);
        AddExpense EditExpense(string expenseId);
        void AddExpense(AddExpense addExpense);
        List<UserExpense> Dashboard(string email);
        void SettleUp(SettleUp settleUp, string email);
    }
}
