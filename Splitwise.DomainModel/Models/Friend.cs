using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class Friend
    {
        #region Properties

        [Key]
        public string Id { get; set; }


        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User1 { get; set; }


        public string FriendId { get; set; }
        [ForeignKey("FriendId")]
        public ApplicationUser User2 { get; set; }

        #endregion
    }
}
