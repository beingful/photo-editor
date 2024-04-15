using Microsoft.AspNetCore.Mvc;
using PhotoEditor.Effects;
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

            Image<Rgba32> originalImage = await Image.LoadAsync<Rgba32>(memoryStream);

            GaussianBlur blur = new(12);
            AdaptiveGaussianThresholding adaptiveGaussianThresholding = new(blur);
            ScreenBlending screenBlending = new();

            BloomEffect bloomEffect = new(blur, adaptiveGaussianThresholding, screenBlending);

            Image<Rgba32> bloomedImage = bloomEffect.Apply(originalImage);

            return View((Before: originalImage, After: bloomedImage));
        }
    }
}
