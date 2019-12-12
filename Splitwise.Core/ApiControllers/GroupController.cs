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
    [Route("api/Groups")]
    [ApiController]
    public class GroupController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<List<UserNameWithId>> GroupList()
        {
            return await _unitOfWork.Group.GetGroupList();
        }

        [HttpDelete]
        [Route("{groupId}")]
        public async Task<object> DeleteFriend(string groupId)
        {
            await _unitOfWork.Group.RemoveGroup(groupId);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet]
        [Route("expenseList/{groupId}")]
        public async Task<List<ExpenseDetail>> GetGroupExpenseList(string groupId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            var expenseDetail = await _unitOfWork.Group.GetGroupExpenseList(groupId, currentUserId);
            return expenseDetail;
        }

        [HttpPost]
        public async Task<object> AddGroup(GroupAdd groupAdd)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            var check = await _unitOfWork.Group.AddGroup(groupAdd, email);
            await _unitOfWork.Commit();
            if (check == 1)
            {
                await _unitOfWork.Commit();
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }

       

        [HttpGet]
        [Route("{groupId}/edit")]
        public async Task<object> GetGroupDetails(string groupId)
        {
            var groupEdit = _unitOfWork.Group.GetGroupDetails(groupId);
            if (groupEdit != null)
            {
                await _unitOfWork.Commit();
                return Ok(groupEdit);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{groupId}/UserExpense")]
        public async Task<object> GroupUserExpense(string groupId, List<string> users)
        {
            List<UserExpense> userExpenses = await _unitOfWork.Group.GroupUserExpense(groupId, users);
            await _unitOfWork.Commit();
            return Ok(userExpenses);
        }
    }
}