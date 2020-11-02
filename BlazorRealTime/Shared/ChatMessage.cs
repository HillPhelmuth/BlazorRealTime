using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorRealTime.Shared
{
    public class ChatMessage
    {
        public ChatMessage(string sender, string message)
        {
            Sender = sender;
            Message = message;
        }
        public string Sender { get; set; }
        public string Message { get; set; }
    }
}
