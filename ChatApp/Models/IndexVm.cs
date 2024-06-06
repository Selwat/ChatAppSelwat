using System.Collections.Generic;

namespace ChatApp.Models
{
    public class IndexVm
    {
        public string UserName { get; set; }
        public List<ChatMessage> Messages { get; set; }
    }
}