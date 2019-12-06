using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class ApplicationUser : IdentityUser
    {
        #region Properties

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Currency { get; set; }
        public bool IsRegistered { get; set; }

        #endregion
    }
}
