using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;

namespace ChatApp.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly ChatContext _context;

        public ChatHub(ChatContext context)
        {
            _context = context;
        }

        public const string ReceiveMessage = "ReceiveMessage";

       public async Task SendMessage(ChatMessage msg)
        {
            msg.CreatedOn = DateTime.UtcNow;
            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();
            var payload = new
            {
                msg.UserName,
                msg.Message,
                FormattedCreatedOn = msg.CreatedOn.ToString("g", CultureInfo.InvariantCulture)
            };

            await Clients.All.SendAsync(ReceiveMessage, payload);
        }

    }
}