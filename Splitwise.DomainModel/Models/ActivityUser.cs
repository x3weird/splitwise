using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class ActivityUser
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        public string ActivityId { get; set; }
        [ForeignKey("ActivityId")]
        public Activity Activity { get; set; }

        public string ActivityUserId { get; set; }
        [ForeignKey("ActivityUserId")]
        public ApplicationUser User { get; set; }

        public string Log { get; set; }

        #endregion
    }
}
