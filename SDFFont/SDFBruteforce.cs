using System;

namespace SDFFont
{
    class SDFBruteforce : ISDF
    {
        public void Process(Raster input, Raster output, int inSize, int outSize, int distance, int ss)
        {
            for (int y = 0; y < outSize; y++)
                for (int x = 0; x < outSize; x++)
                {
                    bool inner = input.Data[y * ss + ss / 2, x * ss + ss / 2] < 128;
                    var minDistance = inner ? int.MaxValue : int.MinValue;

                    for (int j = -distance; j <= distance; j++)
                        for (int i = -distance; i <= distance; i++)
                        {
                            var xi = x * ss + ss / 2 + i;
                            var yj = y * ss + ss / 2 + j;
                            if (xi < 0 || yj < 0 || xi >= inSize || yj >= inSize)
                                continue;

                            bool isInner = input.Data[yj, xi] < 128;

                            if (inner != isInner)
                            {
                                var ddd = i * i + j * j;
                                if (!inner)
                                    ddd = -ddd;
                                if (inner && ddd < minDistance)
                                    minDistance = ddd;
                                if (!inner && ddd > minDistance)
                                    minDistance = ddd;
                            }
                        }
                    bool minus = minDistance < 0;
                    double val = (Math.Sqrt(Math.Abs((double)minDistance)) / distance);
                    val = ((minus ? -val : val) + 1) / 2;
                    int v = (int)(val * 255);
                    if (v < 0)
                        v = 0;
                    if (v > 255)
                        v = 255;
                    output.Data[y, x] = (byte)v;
                }
        }
    }
}
