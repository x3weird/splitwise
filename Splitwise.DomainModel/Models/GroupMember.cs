using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class GroupMember
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        public string GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Group Group { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        #endregion
    }
}
