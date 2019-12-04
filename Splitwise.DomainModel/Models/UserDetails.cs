using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class UserDetails
    {
        #region Properties

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Number { get; set; }
        public string Currency { get; set; }
        public string Password { get; set; }
        public Boolean IsRegistered { get; set; }

        #endregion
    }
}
