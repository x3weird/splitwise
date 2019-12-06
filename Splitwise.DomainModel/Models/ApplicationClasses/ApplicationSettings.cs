
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Splitwise.Models
{
    public class ApplicationSettings
    {
        #region Properties

        public string JWT_Secret { get; set; }
        public string Client_URL { get; set; }

        #endregion
    }
}
