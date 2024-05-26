namespace PhotoEditor.Effects.Blur;

internal sealed class GaussianKernel
{
    private readonly float _deviation;
    private readonly int _kernelSize;

    public GaussianKernel(int radius = 1)
    {
        _deviation = 0.3f * (radius * 2 * 0.5f - 1) + 0.8f;
        _kernelSize = radius * 2 + 1;
    }

    public float[] CalculateBasis()
    {
        float[] kernel = Calculate();

        Normalize(kernel);

        return kernel;
    }

    private float[] Calculate()
    {
        float[] kernalBasis = new float[_kernelSize];

        int kernelRadius = (_kernelSize - 1) / 2;

        for (int x = 0; x < kernalBasis.Length; x++)
        {
            kernalBasis[x] = CalculateWeight(x - kernelRadius);
        }

        return kernalBasis;
    }

    private void Normalize(float[] basis)
    {
        float totalWeight = basis.Sum();

        for (int i = 0; i < _kernelSize; i++)
        {
            basis[i] /= totalWeight;
        }
    }

    private float CalculateWeight(int position)
    {
        double exponent = position * position / (2 * _deviation * _deviation);
        double gaussianNumerator = Math.Exp(-exponent);
        double gaussianDenominator = 2 * Math.PI * _deviation * _deviation;

        return (float)(gaussianNumerator / gaussianDenominator);
    }
}
