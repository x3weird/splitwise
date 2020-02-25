using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models.ApplicationClasses
{
    public class ExpenseNotification
    {
        public string Id { get; set; }
        public string Payload { get; set; }
        public string UserId { get; set; }
        public string Detail { get; set; }
        public string ConnectionId { get; set; }
        public string NotificationOn { get; set; }
        public string NotificationOnId { get; set; }
        public string Severity { get; set; }
    }
}
