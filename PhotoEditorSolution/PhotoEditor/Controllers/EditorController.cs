using Microsoft.AspNetCore.Mvc;
using PhotoEditor.Effects;
using PhotoEditor.Effects.Models;
using PhotoEditor.Interface.Models.Requests;
using PhotoEditor.Models.Requests;
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
        public async Task<IActionResult> EditAsync([FromForm]ImageToEditRequest imageRequest, [FromBody]BloomConfigurationRequest bloomConfigurationRequest)
        {
            using var memoryStream = new MemoryStream();

            await imageRequest.File.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            Image<Rgba32> originalImage = await Image.LoadAsync<Rgba32>(memoryStream);

            BloomEffect bloomEffect = new(new BloomConfiguration
            {
                BlurRadius = bloomConfigurationRequest.BlurRadius,
                DownscalingRatio = bloomConfigurationRequest.DownscalingRatio,
                Intensity = bloomConfigurationRequest.Intensity,
                Threashold = bloomConfigurationRequest.Threashold
            });

            Image<Rgba32> bloomedImage = bloomEffect.Apply(originalImage);

            return View((Before: originalImage, After: bloomedImage));
        }
    }
}
