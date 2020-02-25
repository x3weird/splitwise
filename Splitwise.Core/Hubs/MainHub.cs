using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Splitwise.DomainModel.Models;
using Splitwise.DomainModel.Models.ApplicationClasses;
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

        private IUnitOfWork _unitOfWork;

        public MainHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        public async Task ExpenseNotification()
        {
            string connectionId = Context.ConnectionId;
            Expense expense = new Expense();
            await Clients.Client(connectionId).SendAsync("RecieveMessage", expense);
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.Identity.Name != null)
            {
                NotificationHub notificationHub = new NotificationHub
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = Context.User.Identity.Name
                };
                await _unitOfWork.Notification.AddConnectedUser(notificationHub);
                List<ExpenseNotification> NotificationUser = await _unitOfWork.Notification.GetNotificationUser();
           
                foreach (var item in NotificationUser)
                {
                    if(item.UserId == notificationHub.UserId)
                    {
                        await Clients.Client(item.ConnectionId).SendAsync("RecieveMessage", NotificationUser);
                    }
                }
                await _unitOfWork.Commit();
            }

            if (Context.User.Identity.Name != null)
            {
                ExpenseNotification notificationHub2 = new ExpenseNotification
                {
                    Detail = "Expense",
                    Severity = "success",
                    ConnectionId = Context.ConnectionId,
                    UserId = Context.User.Identity.Name
                };

                ExpenseNotification notificationHubEx = new ExpenseNotification
                {
                    Detail = "Expense",
                    Severity = "success",
                    ConnectionId = Context.ConnectionId,
                    UserId = Context.User.Identity.Name
                };

                NotificationHub notificationHub = new NotificationHub()
                {
                    UserId = Context.User.Identity.Name,
                    ConnectionId = Context.ConnectionId
                };

                NotificationHub notificationHubTest = new NotificationHub()
                {
                    UserId = Context.User.Identity.Name,
                    ConnectionId = Context.ConnectionId
                };

                await _unitOfWork.Notification.AddConnectedUser(notificationHub);
                List<ExpenseNotification> NotificationUser = await _unitOfWork.Notification.GetNotificationUser();

                foreach (var item in NotificationUser)
                {
                    if (item.UserId == notificationHub.UserId)
                    {
                        await Clients.Client(item.ConnectionId).SendAsync("RecieveMessage", NotificationUser);
                    }
                }
                await _unitOfWork.Commit();
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            NotificationHub notificationHub = new NotificationHub
            {
                ConnectionId = Context.ConnectionId,
                UserId = Context.User.Identity.Name
            };
            await _unitOfWork.Notification.RemoveConnectedUser(notificationHub.UserId);
            await _unitOfWork.Commit();
        }
        
    }
}