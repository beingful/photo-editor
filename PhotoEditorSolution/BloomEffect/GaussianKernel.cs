using System.Configuration;

namespace PhotoEditor.Effects;

internal sealed class GaussianKernel
{
    private readonly float _deviation;
    private readonly int _kernelSize;

    [IntegerValidator(MinValue = 1, MaxValue = 10)]
    private int Radius { get; init; } = 1;

    public GaussianKernel(int radius)
    {
        Radius = radius;
        _deviation = Math.Max(radius / 2, 1);
        _kernelSize = radius * 2 + 1;
    }

    public float[][] Calculate()
    {
        float[][] kernel = CalculteKernel();

        NormalizeKernel(kernel);

        return kernel;
    }

    private float[][] CalculteKernel()
    {
        float[][] weights = new float[_kernelSize][];

        for (int y = 0; y < weights.Length; y++)
        {
            weights[y] = new float[_kernelSize];

            for (int x = 0; x < weights[y].Length; x++)
            {
                weights[y][x] = CalculateWeight(y - Radius, x - Radius);
            }
        }

        return weights;
    }

    private void NormalizeKernel(float[][] weights)
    {
        float totalWeight = weights.Sum(values => values.Sum());

        for (int y = 0; y < weights.Length; y++)
        {
            for (int x = 0; x < weights[y].Length; x++)
            {
                weights[y][x] /= totalWeight;
            }
        }
    }

    private float CalculateWeight(float x, float y)
    {
        double exponent = ((x * x) + (y * y)) / (2 * _deviation * _deviation);
        double gaussianNumerator = Math.Exp(-exponent);
        double gaussianDenominator = 2 * Math.PI * _deviation * _deviation;

        return (float)(gaussianNumerator / gaussianDenominator);
    }
}
