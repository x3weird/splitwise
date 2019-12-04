using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class Ledger
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        [Required]
        public string ExpenseId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public float CreditedAmount { get; set; }
        [Required]
        public float DebitedAmount { get; set; }

        #endregion
    }
}
