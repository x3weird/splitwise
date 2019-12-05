using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class Comment
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public string ExpenseId { get; set; }
        [ForeignKey("ExpenseId")]
        public Expense Expense { get; set; }

        [Required]
        public string CommentData { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}
