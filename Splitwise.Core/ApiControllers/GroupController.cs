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
        public List<UserNameWithId> GroupList()
        {
            return _unitOfWork.Group.GetGroupList();
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
        public object AddGroup(GroupAdd groupAdd)
        {
            if(_unitOfWork.Group.AddGroup(groupAdd)==1)
            {
                _unitOfWork.Commit();
                return Ok();
            } else
            {
                return Conflict();
            }
        }

        [HttpPut]
        [Route("{groupId}/edit")]
        public object EditGroup(string groupId, GroupAdd groupAdd)
        {
            if (_unitOfWork.Group.EditGroup(groupId, groupAdd) == 1)
            {
                _unitOfWork.Commit();
                return Ok();
            }
            else
            {
                return Conflict();
                
            }
        }

        [HttpGet]
        [Route("{groupId}/edit")]
        public object GetGroupDetails(string groupId)
        {
            var groupEdit = _unitOfWork.Group.GetGroupDetails(groupId);
            if (groupEdit != null)
            {
                _unitOfWork.Commit();
                return Ok(groupEdit);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{groupId}/UserExpense")]
        public object GroupUserExpense(string groupId, List<string> users)
        {
            List<UserExpense> userExpenses = _unitOfWork.Group.GroupUserExpense(groupId, users);
            return Ok(userExpenses);
        }
    }
}