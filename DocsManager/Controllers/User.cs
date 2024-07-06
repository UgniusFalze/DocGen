using System.Security.Claims;
using DocsManager.Models.Dto;
using DocsManager.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class User(IUserService userService) : ControllerWithUser
{

    [HttpGet("valid")]
    public async Task<ActionResult<bool>> ValidateRegisteredUser()
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");

        return await userService.ValidateUser(user.Value);
    }

    [HttpPost]
    public async Task<ActionResult> PostUser(UserPostDto userPost)
    {
        var user = GetUserGuid();
        var userName = User.FindFirstValue(ClaimTypes.GivenName);
        var surName = User.FindFirstValue(ClaimTypes.Surname);
        if (user == null || userName == null || surName == null) return NotFound("User not found");

        var result = await userService.InsertUser(user.Value, userPost, userName, surName);
        
        return result ? StatusCode(StatusCodes.Status201Created) : BadRequest("User is already created");
    }

    [HttpPut]
    public async Task<IActionResult> PutUser(UserPostDto user)
    {
        var userGuid = GetUserGuid();
        var userName = User.FindFirstValue(ClaimTypes.GivenName);
        var surName = User.FindFirstValue(ClaimTypes.Surname);
        if (userGuid is null || userName is null || surName is null) return NotFound("User not found");
        var result = await userService.UpdateUser(userGuid.Value, user, userName, surName);
        
        return result ? Ok("User updated") : NotFound("User not found");
    }

    [HttpGet]
    public async Task<ActionResult<Models.User>> GetUser()
    {
        var userGuid = GetUserGuid();
        if (userGuid == null) return NotFound("User not found");
        var result = await userService.GetUser(userGuid.Value);
        if (result == null) return NotFound("User not found");
        return result;
    }
}