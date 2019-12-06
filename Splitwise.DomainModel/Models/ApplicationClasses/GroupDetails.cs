using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class GroupDetails
    {
        #region Properties

        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string AddedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Boolean SimplifyDebts { get; set; }
        public List<GroupUsers> Users { get; set; }

        #endregion
    }
}
