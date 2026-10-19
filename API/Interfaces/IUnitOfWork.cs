using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IMemberRepository memberRepository { get; }
        IMessageRepository messageRepository { get; }
        ILikesRepository likesRepository { get; }
        IPhotoRepository photoRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}