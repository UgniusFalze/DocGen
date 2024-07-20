using System.Security.Claims;
using DocsManager.Controllers.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

[Authorize]
public abstract class ControllerWithUser : ControllerBase
{
    protected BearerUser? GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var firstName = User.FindFirstValue(ClaimTypes.GivenName);
        var lastName = User.FindFirstValue(ClaimTypes.Surname);
        if (userId == null || firstName == null || lastName == null) return null;
        return new BearerUser(Guid.Parse(userId), firstName, lastName);
    }
}