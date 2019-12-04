using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class CommentData
    {
        #region Properties

        public string UserId { get; set; }
        public string ExpenseId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}
