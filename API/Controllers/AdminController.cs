using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using API.Services;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService) : BaseApiController
    {
        [Authorize (Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {   
            var users = await userManager.Users.OrderBy(user => user.Email).ToListAsync();
            var userList = new List<object>(); 

            // this method returns anonymous objects, which is fine, but it comes at cost of loosing type safety

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userList.Add(new {
                    user.Id,
                    user.Email,
                    Roles = roles.ToList()
                });
            }

            return Ok(userList);
        }

        [Authorize (Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userId}")]
        public async Task<ActionResult<IList<string>>> EditRoles(string userId, [FromQuery]string roles)
        {
            if(string.IsNullOrEmpty(userId)) return BadRequest("Invalid user id");
            if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

            var selectedRoles = roles.Split(',').ToArray();

            var user = await userManager.FindByIdAsync(userId);

            if(user is null) return BadRequest("Could not retrieve user");

            var userRoles = await userManager.GetRolesAsync(user);

            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await userManager.GetRolesAsync(user));
        }

        [Authorize (Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            return Ok(await unitOfWork.photoRepository.GetUnapprovedPhotos());
        }

        [Authorize (Policy = "ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            var photo = await unitOfWork.photoRepository.GetPhotoById(photoId);

            if (photo is null) return BadRequest("Could not retrieve the photo from DB");

            var member = await unitOfWork.memberRepository.GetMemberByPhotoId(photoId);

            if (member is null) return BadRequest("Could not retrieve the member from DB");

            photo.isApproved = true;

            if (member.ImageUrl is null)
            {
                member.ImageUrl = photo.Url;
                member.User.ImageUrl = photo.Url;
            }

            await unitOfWork.Complete();
            
            return Ok();
        }

        [Authorize (Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId) // delete someone's photo effectively
        {
            var photo = await unitOfWork.photoRepository.GetPhotoById(photoId);

            if (photo is null) return BadRequest("Could not retrieve the photo from DB");

            if (photo.PublicId is not null)
            {
                var deletion = await photoService.DeletePhotoAsync(photo.PublicId);

                if (deletion.Result == "ok") unitOfWork.photoRepository.DeletePhoto(photo);
            }
            else
            {
                unitOfWork.photoRepository.DeletePhoto(photo);
            }            

            await unitOfWork.Complete();
            
            return Ok();
        }
    }
}