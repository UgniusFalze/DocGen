using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

[Authorize]
public abstract class ControllerWithUser : ControllerBase
{
    protected Guid? GetUserGuid()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null) return null;

        return Guid.Parse(user);
    }
}