using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class ExpenseLedger
    {
        #region Properties

        public string UserId { get; set; }
        public string Name { get; set; }
        public float Owes { get; set; }
        public float Paid { get; set; }

        #endregion
    }
}
