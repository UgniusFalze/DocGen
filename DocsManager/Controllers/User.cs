using System.Security.Claims;
using DocsManager.Models;
using DocsManager.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class User (DocsManagementContext context): ControllerBase
    {
        
        private Guid? GetUserGuid()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return null;
            }

            return Guid.Parse(user);
        }
        [HttpGet("valid")]
        public async Task<ActionResult<bool>> ValidateRegisteredUser()
        {
            var user = GetUserGuid();
            if (user == null)
            {
                return NotFound();
            }

            return await context.Users.AnyAsync(cxuser => cxuser.UserId == user);
        }

        [HttpPost]
        public async Task<ActionResult> PostUser(UserPostDto userPost)
        {
            var user = GetUserGuid();
            var userName = User.FindFirstValue(ClaimTypes.GivenName);
            var surName = User.FindFirstValue(ClaimTypes.Surname);
            if (user == null || userName == null || surName == null)
            {
                return NotFound();
            }
            context.Users.Add(new Models.User
            {
                Address = userPost.Address,
                BankName = userPost.BankName,
                BankNumber = userPost.BankNumber,
                FirstName = userName,
                LastName = surName,
                FreelanceWorkId = userPost.FreelanceWorkId,
                PersonalId = userPost.PersonalId,
                UserId = user.Value
            });

            await context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
