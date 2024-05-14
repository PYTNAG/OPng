namespace OPng.Raw;

public class Image
{
    private readonly Sample[][,] _channels;
    public Sample[,] Red => _channels[0];
    public Sample[,] Green => _channels[1];
    public Sample[,] Blue => _channels[2];
    public Sample[,] Alpha => _channels.Length == 4 ? _channels[3] : new Sample[0, 0];

    private Sample.Depth? _sampleDepth;
    public Sample.Depth SampleDepth 
    {
        get
        {
            _sampleDepth ??= _channels[0][0, 0].DepthInBits;
            return (Sample.Depth)_sampleDepth;
        }
    }

    private (int, int)? _size;
    public (int width, int height) Size
    {
        get
        {
            _size ??= (_channels[0].GetLength(0), _channels[0].GetLength(1));
            return ((int, int))_size;
        }
    }

    public Image(
        byte[] data, (int w, int h) size,
        Sample.Depth sampleDepth, bool withAlpha)
    {
        if (data.Length * 8 % (int)sampleDepth != 0)
        {
            throw new Exception("Sample depth is not a factor of data length.");
        }

        _channels = new Sample[withAlpha ? 4 : 3][,];
        for (int i = 0; i < _channels.Length; ++i)
        {
            _channels[i] = new Sample[size.w, size.h];
        }

        for (int sampleIndex = 0; sampleIndex < data.Length * 8 / (int)sampleDepth; ++sampleIndex)
        {
            Span<byte> sampleBuffer;

            if (sampleDepth == Sample.Depth.Bytes2)
            {
                sampleBuffer = data.AsSpan()[(2 * sampleIndex)..(2 * sampleIndex + 1)];
            }
            else
            {
                int bitPos = sampleIndex * (int)sampleDepth;
                int @byte = data[bitPos / 8];
                int bit = bitPos % 8;
                sampleBuffer = new([(byte) (@byte >> (8 - bit - (int)sampleDepth))]);
            }

            _channels[sampleIndex % (withAlpha ? 4 : 3)][sampleIndex % size.w, sampleIndex / size.w] 
                    = new Sample(BitConverter.ToUInt16(sampleBuffer), sampleDepth);
        }
    }

    public (int r, int g, int b, int? a) this[int x, int y]
    {
        get
        {
            if (x >= Size.width || y >= Size.height)
            {
                throw new IndexOutOfRangeException($"Out of image size {Size.width}x{Size.height}.");
            }

            int r = _channels[0][x, y].Value;
            int g = _channels[1][x, y].Value;
            int b = _channels[2][x, y].Value;
            int? a = _channels.Length == 4 ? _channels[4][x, y].Value : null;

            return (r, g, b, a);
        }

        set
        {
            if (x >= Size.width || y >= Size.height)
            {
                throw new IndexOutOfRangeException($"Out of image size {Size.width}x{Size.height}.");
            }
            
            if (value.r > (1 << (int)SampleDepth) 
                || value.g > (1 << (int)SampleDepth)
                || value.b > (1 << (int)SampleDepth)
                || value.a is not null && value.a > (1 << (int)SampleDepth))
            {
                throw new Exception($"Sample overflow. Pixel's samples depth is {(int)SampleDepth} bit(s).");
            }

            _channels[0][x, y] = new(value.r, SampleDepth);
            _channels[1][x, y] = new(value.g, SampleDepth);
            _channels[2][x, y] = new(value.b, SampleDepth);

            if (value.a is not null && _channels.Length == 4)
            {
                _channels[3][x, y] = new((int)value.a, SampleDepth);
            }
        }
    }
}