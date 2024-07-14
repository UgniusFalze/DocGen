using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

public class ErrorController : ControllerBase
{
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleError()
    {
        return Problem();
    }
}