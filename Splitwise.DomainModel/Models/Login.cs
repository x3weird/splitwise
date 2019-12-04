using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class Login
    {
        #region Properties

        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        #endregion
    }
}
