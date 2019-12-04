using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class Friend
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [Required]
        public string FriendId { get; set; }

        #endregion
    }
}
