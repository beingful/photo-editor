using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PhotoEditor.Interface.ViewModels;

namespace PhotoEditor.Interface.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            IExceptionHandlerPathFeature? exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            return View(new ErrorViewModel(exceptionHandlerPathFeature?.Error.Message ?? "Unhandled error"));
        }
    }
}
