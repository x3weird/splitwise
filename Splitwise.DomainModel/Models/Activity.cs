using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class Activity
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        [Required]
        public string Log { get; set; }
        [Required]
        public string ActivityOn { get; set; }
        [Required]
        public string ActivityOnId { get; set; }

        #endregion
    }
}
