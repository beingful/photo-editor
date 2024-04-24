namespace PhotoEditor.Effects.Models;

public sealed class BloomConfiguration
{
    public int BlurRadius { get; init; }

    public int DownscalingRatio { get; init; }

    public float Intensity { get; init; }

    public byte Threashold { get; init; }
}
