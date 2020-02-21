using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models.ApplicationClasses
{
    public class ExpenseNotification
    {
        public string Payload { get; set; }
        public string UserId { get; set; }
        public string Detail { get; set; }
        public string ConnectionId { get; set; }
    }
}
