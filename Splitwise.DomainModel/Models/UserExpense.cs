using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class UserExpense
    {
        #region Properties

        public string Name { get; set; }
        public string Id { get; set; }
        public float Amount { get; set; }

        #endregion
    }
}
