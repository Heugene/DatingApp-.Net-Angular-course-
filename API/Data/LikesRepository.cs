using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        public Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
        {
            throw new NotImplementedException();
        }

        public Task<MemberLike> GetMemberLike(string sourceMemberId, string targetMemberId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Member>> GetMemberLikes(string predicate, string memberId)
        {
            throw new NotImplementedException();
        }

        public void Like(MemberLike like)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAllChanges()
        {
            throw new NotImplementedException();
        }

        public void Unlike(MemberLike like)
        {
            throw new NotImplementedException();
        }
    }
}