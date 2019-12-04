using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class Comment
    {
        #region Properties

        [Key]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [Required]
        public string ExpenseId { get; set; }
        [Required]
        public string CommentData { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}
