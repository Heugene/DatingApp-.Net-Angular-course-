using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace API.SignalR
{
    public class MessageHub(IMessageRepository messageRepository) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var otherUserId = httpContext?.Request?.Query["userId"].ToString()
                ?? throw new HubException("Other user not found");
            
            var groupName = GetGroupName(GetUserId(), otherUserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = messageRepository.GetMessageThread(GetUserId(), otherUserId);

            await Clients.Group(groupName).SendAsync("RecieveMessageThread", messages);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        private static string GetGroupName(string? caller, string? other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}={other}" : $"{other}={caller}";
        }

        private string GetUserId()
        {
            return Context.User?.GetMemberId() ?? throw new HubException("Cannot get member id");
        }
    }
}