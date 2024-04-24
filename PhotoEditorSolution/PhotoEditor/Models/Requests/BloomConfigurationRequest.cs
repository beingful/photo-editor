namespace PhotoEditor.Interface.Models.Requests;

public sealed class BloomConfigurationRequest
{
    public int BlurRadius { get; init; }

    public int DownscalingRatio { get; init; }

    public float Intensity { get; init; }

    public byte Threashold { get; init; }
}
