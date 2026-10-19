using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var sender = await unitOfWork.memberRepository.GetMemberByIdAsync(User.GetMemberId());
            var recipient = await unitOfWork.memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

            if (sender is null || recipient is null || sender.Id == createMessageDto.RecipientId)
            {
                return BadRequest("Cannot send this message");
            }

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content
            };

            unitOfWork.messageRepository.AddMessage(message);

            if (await unitOfWork.Complete()) return message.ToDto();

            return BadRequest("Failed to save the message");
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessagesByContainer(
            [FromQuery] MessageParams messageParams)
        {
            messageParams.MemberId = User.GetMemberId();
            return await unitOfWork.messageRepository.GetMessagesForMember(messageParams);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
        {
            return Ok(await unitOfWork.messageRepository.GetMessageThread(User.GetMemberId(), recipientId));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(string id) 
        { // both users have to use this to actually delete a message from DB
            var memberId = User.GetMemberId();

            var message = await unitOfWork.messageRepository.GetMessage(id);

            if (message is null) return BadRequest("Cannot delete this message");
            
            if (message.SenderId != memberId && message.RecipientId != memberId)
                return BadRequest("You cannot delete this message");
            
            if (message.SenderId == memberId) message.SenderDeleted = true;
            if (message.RecipientId == memberId) message.RecipientDeleted = true;

            if(message is {SenderDeleted: true, RecipientDeleted: true})
            {
                unitOfWork.messageRepository.DeleteMessage(message);
            }
            
            if (await unitOfWork.Complete()) return Ok();

            return BadRequest("Problem with message deletion");
        }
    }
}