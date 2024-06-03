using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidClient.Models
{
    public class SendMessage
    {
        public Guid ChatId { get; set; }
        public string TextToSend { get; set; }
    }
}
