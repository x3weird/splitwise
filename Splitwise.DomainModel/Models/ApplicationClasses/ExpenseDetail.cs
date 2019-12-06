using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class ExpenseDetail
    {
        #region Properties

        public string Description { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string ExpenseId { get; set; }
        public string ExpenseType { get; set; }
        public string Note { get; set; }
        public DateTime CreatedOn { get; set; }
        public string AddedBy { get; set; }
        public float Amount { get; set; }
        public List<ExpenseLedger> ExpenseLedgers { get; set; }
        public List<string> Comment { get; set; }

        #endregion
    }
}
