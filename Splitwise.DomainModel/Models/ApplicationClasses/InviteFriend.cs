using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class InviteFriend
    {
        #region Properties

        public string Message { get; set; }
        public List<string> Email { get; set; }

        #endregion
    }
}
