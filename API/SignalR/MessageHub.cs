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
    public class MessageHub(IMessageRepository messageRepository, IMemberRepository memberRepository) : Hub
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

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
                var group = GetGroupName(sender.Id, recipient.Id);
                await Clients.Group(group).SendAsync("NewMessage", message.ToDto());
            }
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