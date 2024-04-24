using System.Numerics;

namespace PhotoEditor.Effects;

internal sealed class GaussianKernel
{
    private readonly float _deviation;
    private readonly int _kernelSize;

    public GaussianKernel(int radius = 1)
    {
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

        int kernelRadius = (_kernelSize - 1) / 2;

        for (int x = 0; x < weights.Length; x++)
        {
            weights[x] = new float[_kernelSize];

            for (int y = 0; y < weights[x].Length; y++)
            {
                weights[x][y] = CalculateWeight(new Vector2(x - kernelRadius, y - kernelRadius));
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

    private float CalculateWeight(Vector2 position)
    {
        double exponent = ((position.X * position.X) + (position.Y * position.Y)) / (2 * _deviation * _deviation);
        double gaussianNumerator = Math.Exp(-exponent);
        double gaussianDenominator = 2 * Math.PI * _deviation * _deviation;

        return (float)(gaussianNumerator / gaussianDenominator);
    }
}
