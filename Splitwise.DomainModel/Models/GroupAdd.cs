using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class GroupAdd
    {
        #region Properties

        public string Name { get; set; }
        public string AddedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Boolean SimplifyDebts { get; set; }
        public List<string> Users { get; set; }

        #endregion
    }
}
