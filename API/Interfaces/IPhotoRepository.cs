using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IReadOnlyList<PhotoForApprovalDto>> GetUnapprovedPhotos();

        Task<Photo?> GetPhotoById(int id);
        void DeletePhoto(Photo photo);
    }
}