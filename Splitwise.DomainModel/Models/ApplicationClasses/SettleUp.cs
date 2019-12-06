using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class SettleUp
    {
        public float Amount { get; set; }
        public DateTime Date { get; set; }
        public string Group { get; set; }
        public string Note { get; set; }
        public string Payer { get; set; }
        public string Recipient { get; set; }
    }
}
