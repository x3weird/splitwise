using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class GroupUsers
    {
        #region Properties

        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        
        #endregion
    }
}
