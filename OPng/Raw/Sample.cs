namespace OPng.Raw;

public sealed class Sample
{
    public enum Depth
    {
        Bit = 1,
        Bits2 = 2,
        Bits4 = 4,
        Byte = 8,
        Bytes2 = 16
    }

    public Depth DepthInBits { get; }
    public int Value { get; set; }

    public Sample(int value, Depth depth)
    {
        if (value >= (1 << (int)depth))
        {
            throw new Exception($"Value cannot be greater than {(1 << (int)depth) - 1}");
        }

        Value = value;
        DepthInBits = depth;
    }
}