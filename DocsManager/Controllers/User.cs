using System.Security.Claims;
using DocsManager.Models.Dto;
using DocsManager.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class User(IUserService userService) : ControllerWithUser
{
    /// <summary>
    /// Validates if user has finished setting up their profile
    /// </summary>
    /// <returns>Has user finished setting up</returns>
    /// <response code="200">Returns has user finished setting up profile</response>
    /// <response code="404">If user is not found</response>
    [HttpGet("valid")]
    public async Task<ActionResult<bool>> ValidateRegisteredUser()
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");

        return await userService.ValidateUser(user.Value);
    }
    
    /// <summary>
    /// Inserts new user profile
    /// </summary>
    /// <param name="userPost">User profile</param>
    /// <returns></returns>
    /// <response code="201">New user profile has been created</response>
    /// <response code="400">User profile already exists</response>
    /// <response code="404">If user is not found (from authentication service)</response>
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
    
    /// <summary>
    /// Updates users profile
    /// </summary>
    /// <param name="user"></param>
    /// <response code="204">User profile has been updated</response>
    /// <response code="404">If user is not found</response>
    [HttpPut]
    public async Task<IActionResult> PutUser(UserPostDto user)
    {
        var userGuid = GetUserGuid();
        var userName = User.FindFirstValue(ClaimTypes.GivenName);
        var surName = User.FindFirstValue(ClaimTypes.Surname);
        if (userGuid is null || userName is null || surName is null) return NotFound("User not found");
        var result = await userService.UpdateUser(userGuid.Value, user, userName, surName);
        
        return result ? NoContent() : NotFound("User not found");
    }
    
    /// <summary>
    /// Returns current users profile
    /// </summary>
    /// <returns>Users profile</returns>
    /// <response code="200">Returns users profile</response>
    /// <response code="404">User is not found</response>
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