using System;
using System.Collections.Generic;
using System.Text;

namespace JukeBox.Core
{
    public class nexmo_inbound_info
    {
        public string Type { get; set; }
        public string To { get; set; }
        public string Msisdn { get; set; }
        public string MessageId { get; set; }
        public string MessageTimestamp { get; set; }
    }
}
