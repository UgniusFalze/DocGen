using System.Security.Claims;
using DocsManager.Models;
using DocsManager.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class User(DocsManagementContext context) : ControllerBase
{
    private Guid? GetUserGuid()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null) return null;

        return Guid.Parse(user);
    }

    [HttpGet("valid")]
    public async Task<ActionResult<bool>> ValidateRegisteredUser()
    {
        var user = GetUserGuid();
        if (user == null) return NotFound();

        return await context.Users.AnyAsync(cxuser => cxuser.UserId == user);
    }

    [HttpPost]
    public async Task<ActionResult> PostUser(UserPostDto userPost)
    {
        var user = GetUserGuid();
        var userName = User.FindFirstValue(ClaimTypes.GivenName);
        var surName = User.FindFirstValue(ClaimTypes.Surname);
        if (user == null || userName == null || surName == null) return NotFound();

        if (context.Users.Any(u => u.UserId == user)) return BadRequest();

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

    [HttpPut]
    public async Task<IActionResult> PutUser(UserPostDto user)
    {
        var userGuid = GetUserGuid();
        var userName = User.FindFirstValue(ClaimTypes.GivenName);
        var surName = User.FindFirstValue(ClaimTypes.Surname);
        if (userGuid is null || userName is null || surName is null) return NotFound();
        var userId = userGuid ?? throw new NullReferenceException();
        var modifiedUser = new Models.User
        {
            UserId = userId,
            FirstName = userName,
            LastName = surName,
            Address = user.Address,
            BankName = user.BankName,
            BankNumber = user.BankNumber,
            FreelanceWorkId = user.FreelanceWorkId,
            PersonalId = user.PersonalId
        };

        context.Entry(modifiedUser).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(userId))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<Models.User>> GetUser()
    {
        var userGuid = GetUserGuid();
        if (userGuid == null) return NotFound();
        var user = await context.Users.FindAsync(userGuid);
        
        if (user == null) return NotFound();

        return user;
    }

    private bool UserExists(Guid id)
    {
        return context.Users.Any(e => e.UserId == id);
    }
}