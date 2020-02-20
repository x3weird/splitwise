using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class NotificationHub
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
    }
}
