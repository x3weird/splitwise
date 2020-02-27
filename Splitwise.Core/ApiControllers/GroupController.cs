using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Splitwise.Core.Hubs;
using Splitwise.DomainModel.Models;
using Splitwise.DomainModel.Models.ApplicationClasses;
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
        private readonly IHubContext<MainHub> _mainHub;

        public GroupController(IUnitOfWork unitOfWork, IHubContext<MainHub> mainHub)
        {
            _unitOfWork = unitOfWork;
            _mainHub = mainHub;
        }

        [HttpGet]
        public async Task<List<UserNameWithId>> GroupList()
        {
            return await _unitOfWork.Group.GetGroupList();
        }

        [HttpDelete]
        [Route("{groupId}")]
        public async Task<object> DeleteGroup(string groupId)
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
            Group group = await _unitOfWork.Group.AddGroup(groupAdd, email);
            await _unitOfWork.Commit();
            var check = await _unitOfWork.Group.AddGroupMembers(groupAdd, email, group);
            await _unitOfWork.Commit();
            if (check == 1)
            {
                
                List<NotificationHub> connectedUsers = await _unitOfWork.Notification.GetConnectedUser();
                
                int flag = 0;
                foreach (var item in groupAdd.Users)
                {
                    
                    Notification notification = new Notification()
                    {
                        Payload = group.Name,
                        Detail = "Your Are Added In Group",
                        NotificationOn = "Group",
                        NotificationOnId = group.Id,
                        Severity = "success",
                        Email = item.Email
                    };
                    foreach (var user in connectedUsers)
                    {
                        flag = 0;
                        if (item.Email.ToLower() == user.Email.ToLower() && item.Email.ToLower() != email.ToLower())
                        {
                            flag = 1;
                            await _mainHub.Clients.Client(user.ConnectionId).SendAsync("RecieveMessage", notification);
                        }
                    }

                    if (item.Email.ToLower() != email.ToLower() && flag == 0)
                    {
                        await _unitOfWork.Notification.AddNotificationUser(notification);
                        await _unitOfWork.Commit();
                    }
                }
                
                return Ok();
                
            }
            else
            {
                return Conflict();
            }
        }

    }
}