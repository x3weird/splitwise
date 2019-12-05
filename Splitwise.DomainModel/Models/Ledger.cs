using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class Ledger
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        public string ExpenseId { get; set; }
        [ForeignKey("ExpenseId")]
        public Expense Expense { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public float CreditedAmount { get; set; }
        [Required]
        public float DebitedAmount { get; set; }

        #endregion
    }
}
