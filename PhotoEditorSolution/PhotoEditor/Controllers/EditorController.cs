using BloomEffect;
using Microsoft.AspNetCore.Mvc;
using PhotoEditor.Models.Request;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace PhotoEditor.Controllers
{
    public class EditorController : Controller
    {
        private readonly ILogger<EditorController> _logger;

        public EditorController(ILogger<EditorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult StartAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync([FromForm]ImageToEditRequest imageRequest)
        {
            using var memoryStream = new MemoryStream();

            await imageRequest.File.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            GaussianBlur blur = new(15);

            Image<Rgba32> originalImage = await Image.LoadAsync<Rgba32>(memoryStream);
            Image imageBlurred = blur.ApplyAsync(originalImage);

            return View((Before: (Image)originalImage, After: imageBlurred));
        }
    }
}
