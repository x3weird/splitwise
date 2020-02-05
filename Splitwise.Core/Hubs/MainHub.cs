using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Core.Hubs
{
    public class MainHub : Hub<ITypedHubClient>
    {
        //public async Task NewMessage(string msg)
        //{
        //    await Clients.All.SendAsync("MessageReceived", msg);
        //}
    }
}