using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace API.SignalR
{
    public class MessageHub(IMessageRepository messageRepository, 
        IMemberRepository memberRepository, IHubContext<PresenceHub> presenceHub) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var otherUserId = httpContext?.Request?.Query["userId"].ToString()
                ?? throw new HubException("Other user not found");
            
            var groupName = GetGroupName(GetUserId(), otherUserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);

            var messages = await messageRepository.GetMessageThread(GetUserId(), otherUserId);

            await Clients.Group(groupName).SendAsync("RecieveMessageThread", messages);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var sender = await memberRepository.GetMemberByIdAsync(GetUserId());
            var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

            if (sender is null || recipient is null || sender.Id == createMessageDto.RecipientId)
            {
                throw new HubException("Cannot send this message");
            }

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.Id, recipient.Id);
            var group = await messageRepository.GetMessageGroup(groupName);
            var isRecipientInGroup = group is not null && group.Connections.Any(x => x.UserId == message.RecipientId);

            if (isRecipientInGroup)
            {
                message.ReadDateTime = DateTime.UtcNow;
            }

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", message.ToDto());
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.Id);
                if (connections is not null && connections.Count > 0 && !isRecipientInGroup)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageRecieved", message.ToDto());
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await messageRepository.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task<bool> AddToGroup(string groupName)
        {
            var group = await messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, GetUserId());

            if (group is null)
            {
                group = new Group(groupName);
                messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            return await messageRepository.SaveAllAsync();
        }

        private static string GetGroupName(string? caller, string? other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private string GetUserId()
        {
            return Context.User?.GetMemberId() ?? throw new HubException("Cannot get member id");
        }
    }
}