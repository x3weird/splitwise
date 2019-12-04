using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class Expense
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public float Amount { get; set; }
        [Required]
        public string ExpenseType { get; set; }
        public string Note { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        [Required]
        public string AddedBy { get; set; }
        [Required]
        public Boolean IsDeleted { get; set; }

        #endregion
    }
}
