using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class ActivityUser
    {
        #region Properties

        [Key]
        public string Id { get; set; }
        public string ActivityId { get; set; }
        public string ActivityUserId { get; set; }
        public string Log { get; set; }

        #endregion
    }
}
