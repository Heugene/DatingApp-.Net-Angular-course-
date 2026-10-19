using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository(AppDbContext context) : IPhotoRepository
    {
        public void DeletePhoto(Photo photo)
        {
            context.Photos.Remove(photo);
        }

        public async Task<Photo?> GetPhotoById(int id)
        {
            return await context.Photos.IgnoreQueryFilters().SingleOrDefaultAsync(photo => photo.Id == id);
        }

        public async Task<IReadOnlyList<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            var result = context.Photos.IgnoreQueryFilters().Where(p => p.isApproved == false)
                .Select(photo => new PhotoForApprovalDto
                {
                    Id = photo.Id,
                    Url = photo.Url,
                    memberId = photo.MemberId,
                    isApproved = photo.isApproved
                });

            return await result.ToListAsync();
        }
    }
}