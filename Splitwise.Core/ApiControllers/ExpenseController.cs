using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Core.ApiControllers
{
    [Route("api/expenses")]
    [ApiController]
    public class ExpenseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("addExpense")]
        public async Task<object> AddExpense(AddExpense expense)
        {
            await _unitOfWork.Expense.AddExpense(expense);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpPost]
        [Route("settleUp")]
        public async Task<object> SettleUp(SettleUp settleUp)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            await _unitOfWork.Expense.SettleUp(settleUp, email);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet]
        public async Task<List<ExpenseDetail>> GetExpenseList()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            var expenseDetailList = await _unitOfWork.Expense.GetExpenseList(email);
            return expenseDetailList;
        }

        public async Task<object> DeleteExpense(string expenseId)
        {
            int i = await _unitOfWork.Expense.DeleteExpense(expenseId);
            if (i == 1)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }

        }

        [HttpGet]
        [Route("dashboard")]
        public async Task<object> Dashboard()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            return await _unitOfWork.Expense.Dashboard(email);
        }
    }
}
