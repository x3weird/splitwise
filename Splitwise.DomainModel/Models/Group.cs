using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class Group
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string AddedBy { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        [Required]
        public Boolean SimplifyDebts { get; set; }
        [Required]
        public Boolean IsDeleted { get; set; }

        #endregion
    }
}
