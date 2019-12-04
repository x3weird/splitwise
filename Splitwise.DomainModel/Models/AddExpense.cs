using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class AddExpense
    {
        #region Properties

        public string Id { get; set; }
        public List<string> EmailList {get; set;}
        public string GroupId { get; set;}
        public float Amount { get; set; }
        public string ExpenseType { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public DateTime CreatedOn { get; set; }
        public string AddedBy { get; set; }
        public List<UserExpenseDetail> PaidBy { get; set; }
        public List<UserExpenseDetail> Ledger { get; set; }

        #endregion
    }
}
