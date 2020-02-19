using Microsoft.AspNetCore.SignalR;
using Splitwise.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Core.Hubs
{
    public class MainHub : Hub
    {
        //public async Task NewMessage(string msg)
        //{
        //    await Clients.All.SendAsync("MessageReceived", msg);
        //}

        //private IUnitOfWork _unitOfWork;

        //public MainHub(IUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //}

        //public async Task ExpenseNotification(string userId, Expense expense)
        //{
        //    string userIdOnline = Context.User.Identity.Name;
        //    string connectionId = Context.ConnectionId;
        //    if (userId == userIdOnline)
        //    {
        //        await Clients.Client(connectionId).SendAsync("RecieveMessage", expense);
        //    }
        //    else
        //    {
        //        await _unitOfWork.Notification.AddNotificationUser(userId, expense.Id);
        //    }
        //}
    }
}