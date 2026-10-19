using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository(AppDbContext context) : IMessageRepository
    {
        public void AddGroup(Group group)
        {
            context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Connection?> GetConnection(string connectionId)
        {
            return await context.Connections.FindAsync(connectionId);
        }

        public async Task<Group?> GetGroupForConnection(string connectionId)
        {
            return await context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message?> GetMessage(string messageId)
        {
            return await context.Messages.FindAsync(messageId);
        }

        public async Task<Group?> GetMessageGroup(string groupName)
        {
            return await context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams)
        {
            var query = context.Messages
                .OrderByDescending(m => m.SentDateTime)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Outbox" => query.Where(x => x.SenderId == messageParams.MemberId && !x.SenderDeleted),
                _ => query.Where(x => x.RecipientId == messageParams.MemberId && !x.RecipientDeleted)
            };

            var messageQuery = query.Select(MessageExtensions.ToDtoProjection());

            return await PaginationHelper.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string otherMemberId)
        {
            await context.Messages
                .Where(x => x.RecipientId == currentMemberId && x.SenderId == otherMemberId && x.ReadDateTime == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ReadDateTime, DateTime.UtcNow));
            
            return await context.Messages
                .Where(x => (x.RecipientId == currentMemberId && !x.RecipientDeleted && x.SenderId == otherMemberId) 
                    || (x.RecipientId == otherMemberId && !x.SenderDeleted && x.SenderId == currentMemberId))
                .OrderBy(x => x.SentDateTime)
                .Select(MessageExtensions.ToDtoProjection())
                .ToListAsync();
        }

        public async Task RemoveConnection(string connectionId)
        {
            await context.Connections.Where(c => c.ConnectionId == connectionId).ExecuteDeleteAsync();
        }
    }
}