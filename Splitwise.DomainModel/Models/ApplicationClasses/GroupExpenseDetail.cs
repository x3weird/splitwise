using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class GroupExpenseDetail
    {
        #region Properties

        public string ExpenseId { get; set; }
        public string ExpenseType { get; set; }
        public string Note { get; set; }
        public DateTime CreatedOn { get; set; }
        public string AddedBy { get; set; }
        public List<ExpenseLedger> GroupExpenseLedgers { get; set; }

        #endregion
    }
}
