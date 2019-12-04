using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class GroupExpense
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        [Required]
        public string GroupId { get; set; }
        [Required]
        public string ExpenseId { get; set; }

        #endregion
    }
}
