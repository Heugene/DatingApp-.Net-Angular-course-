using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository(AppDbContext context) : ILikesRepository
    {
        public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
        {
            return await context.Likes.Where(like => like.SourceMemberId == memberId).Select(x => x.TargetMemberId).ToListAsync();
        }

        public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
        {
            return await context.Likes.FindAsync(sourceMemberId, targetMemberId);
        }

        public async Task<IReadOnlyList<Member>> GetMemberLikes(string predicate, string memberId)
        {
            var query = context.Likes.AsQueryable();

            switch (predicate)
            {
                case "liked": 
                    return await query
                        .Where(x => x.SourceMemberId == memberId)
                        .Select(x => x.TargetMember)
                        .ToListAsync();
                case "likedBy":
                    return await query
                        .Where(x => x.TargetMemberId == memberId)
                        .Select(x => x.SourceMember)
                        .ToListAsync();
                default: // mutual
                    var likeIds = await GetCurrentMemberLikeIds(memberId);
                    return await query
                        .Where(x => x.TargetMemberId == memberId && likeIds.Contains(x.SourceMemberId))
                        .Select(x => x.SourceMember)
                        .ToListAsync();
            }
        }

        public void Like(MemberLike like)
        {
            context.Likes.Add(like);
        }

        public void Unlike(MemberLike like)
        {
            context.Likes.Remove(like);
        }

        public async Task<bool> SaveAllChanges()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}