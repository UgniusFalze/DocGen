using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

public class ErrorController : ControllerBase
{
    [Route("/error")]
    public IActionResult HandleError()
    {
        
        return Problem();
    }
        
}