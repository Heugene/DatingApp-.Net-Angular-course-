using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost("{targetMemberId}")]
        public async Task<ActionResult> ToggleLike(string targetMemberId)
        {
            var sourceMemberId = User.GetMemberId();

            if (sourceMemberId == targetMemberId) return BadRequest("You cannot like yourself");

            var existingLike = await unitOfWork.likesRepository.GetMemberLike(sourceMemberId, targetMemberId);

            if (existingLike is null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId, 
                    TargetMemberId = targetMemberId
                };

                unitOfWork.likesRepository.Like(like);
            }
            else
            {
                unitOfWork.likesRepository.Unlike(existingLike);
            }

            if (await unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to update a like");
        }

        [HttpGet("list")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikeIds()
        {
            return Ok(await unitOfWork.likesRepository.GetCurrentMemberLikeIds(User.GetMemberId()));
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Member>>> GetMemberLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.MemberId = User.GetMemberId();
            var members = unitOfWork.likesRepository.GetMemberLikes(likesParams);
            return Ok(members);
        }
    }
}