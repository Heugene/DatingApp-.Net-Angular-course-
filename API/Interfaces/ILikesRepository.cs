using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId);
        Task<IReadOnlyList<Member>> GetMemberLikes(string predicate, string memberId);
        Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);
        void Unlike(MemberLike like);
        void Like(MemberLike like);
        Task<bool> SaveAllChanges();
    }
}