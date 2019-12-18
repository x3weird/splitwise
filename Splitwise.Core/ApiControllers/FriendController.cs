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
    [Route("api/friends")]
    [ApiController]
    public class FriendController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FriendController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<List<UserNameWithId>> Friend()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            return await _unitOfWork.Friend.GetFriendList(userId);
        }

        [HttpDelete]
        [Route("{friendId}")]
        public async Task<object> DeleteFriend(string friendId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            await _unitOfWork.Friend.RemoveFriend(friendId, userId);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpPost]
        [Route("inviteFriend")]
        public async Task<object> InviteFriend(InviteFriend inviteFriend)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            await _unitOfWork.Friend.RegisterNewFriends(inviteFriend, currentUserId);
            await _unitOfWork.Friend.InviteFriend(inviteFriend, currentUserId);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet]
        [Route("expenseList/{friendId}")]
        public async Task<List<ExpenseDetail>> GetFriendExpenseList(string friendId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            var expenseDetail = await _unitOfWork.Friend.GetFriendExpenseList(friendId, currentUserId);
            return expenseDetail;
        }

        [HttpGet]
        [Route("users/{userId}")]
        public async Task<UserExpense> UserExpense(string userId)
        {
            return await _unitOfWork.Friend.UserExpense(userId);
        }
    }
}
