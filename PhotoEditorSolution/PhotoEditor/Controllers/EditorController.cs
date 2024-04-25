using Microsoft.AspNetCore.Mvc;
using PhotoEditor.Effects;
using PhotoEditor.Effects.Models;
using PhotoEditor.Interface.ViewModels;
using PhotoEditor.Models.Requests;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace PhotoEditor.Controllers;

public class EditorController : Controller
{
    private readonly ILogger<EditorController> _logger;

    public EditorController(ILogger<EditorController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Start()
    {
        return View();
    }

    [HttpPost]
    [Route("preview")]
    public async Task<IActionResult> PreviewImageAsync([FromForm] ImageToEditRequest imageToEditRequest)
    {
        using var memoryStream = new MemoryStream();

        await imageToEditRequest.File.CopyToAsync(memoryStream);

        byte[] image = memoryStream.ToArray();

        BloomConfiguration bloomConfiguration = new()
        {
            BlurRadius = imageToEditRequest.BlurRadius,
            DownscalingRatio = imageToEditRequest.DownscalingRatio,
            Intensity = (float)imageToEditRequest.Intensity,
            Threashold = imageToEditRequest.Threashold
        };

        return View(new PreviewImageViewModel(imageToEditRequest.File, image, bloomConfiguration));
    }

    [HttpPost]
    [Route("edit")]
    public async Task<IActionResult> EditImageAsync([FromForm]ImageToEditRequest imageToEditRequest)
    {
        using var memoryStream = new MemoryStream();

        await imageToEditRequest.File.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        Image<Rgba32> originalImage = await Image.LoadAsync<Rgba32>(memoryStream);

        BloomConfiguration bloomConfiguration = new()
        {
            BlurRadius = imageToEditRequest.BlurRadius,
            DownscalingRatio = imageToEditRequest.DownscalingRatio,
            Intensity = (float)imageToEditRequest.Intensity,
            Threashold = imageToEditRequest.Threashold
        };

        BloomEffect bloomEffect = new(bloomConfiguration);

        Image<Rgba32> bloomedImage = bloomEffect.Apply(originalImage);

        return View(new EditImageViewModel(originalImage, bloomedImage, bloomConfiguration));
    }
}
