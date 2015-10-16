using System;

namespace SDFFont
{
    class SDF
    {
        public Raster Process(Raster input, int inSize, int outSize, int distance, int ss)
        {
            var output = new Raster(outSize);

            for (int y = 0; y < outSize; y++)
                for (int x = 0; x < outSize; x++)
                {
                    bool inner = input.Data[x * ss + ss / 2, y * ss + ss / 2] < 128;
                    var minDistance = inner ? int.MaxValue : int.MinValue;

                    for (int j = -distance; j <= distance; j++)
                        for (int i = -distance; i <= distance; i++)
                        {
                            var xi = x * ss + ss / 2 + i;
                            var yj = y * ss + ss / 2 + j;
                            if (xi < 0 || yj < 0 || xi >= inSize || yj >= inSize)
                                continue;

                            bool isInner = input.Data[xi, yj] < 128;

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
                    output.Data[x, y] = (byte)v;
                }
            return output;
        }

        public Raster Process2(Raster input, int inSize, int outSize, int distance, int ss)
        {
            var output = new Raster(outSize);

            var distx = new short[inSize, inSize];
            var disty = new short[inSize, inSize];

            Process2Forward(input, output, inSize, distance, ss, distx, disty);
            Process2Backward(input, output, inSize, distance, ss, distx, disty);

            return output;
        }

        void Process2Forward(Raster input, Raster output, int inSize, int distance, int ss, short[,] distx, short[,] disty)
        {
            for (int y = 0; y < inSize; y++)
                for (int x = 0; x < inSize; x++)
                {
                    bool inner = input.Data[x, y] < 128;
                    var minDistance = inner ? int.MaxValue : int.MinValue;
                    int mx = -distance;
                    int my = -distance;

                    for (int k = 0; k < 4; k++)
                    {
                        int dx = 0;
                        int dy = 0;
                        switch (k)
                        {
                            case 0: dx = -1; dy = 0; break;
                            case 1: dx = -1; dy = -1; break;
                            case 2: dx = 0; dy = -1; break;
                            case 3: dx = 1; dy = -1; break;
                        }

                        int rx = x + dx;
                        int ry = y + dy;

                        if (rx < 0 || rx >= inSize || ry < 0 || ry >= inSize)
                            continue;

                        bool inner2 = input.Data[rx, ry] < 128;

                        if (inner == inner2)
                        {
                            dx += distx[rx, ry];
                            dy += disty[rx, ry];
                        }

                        var ddd = dx * dx + dy * dy;
                        if (!inner)
                            ddd = -ddd;
                        if (inner && ddd < minDistance)
                        {
                            minDistance = ddd;
                            mx = dx;
                            my = dy;
                        }
                        if (!inner && ddd > minDistance)
                        {
                            minDistance = ddd;
                            mx = dx;
                            my = dy;
                        }
                    }
                    if (mx > short.MaxValue) mx = short.MaxValue;
                    if (mx < short.MinValue) mx = short.MinValue;
                    if (my > short.MaxValue) my = short.MaxValue;
                    if (my < short.MinValue) my = short.MinValue;
                    distx[x, y] = (short)mx;
                    disty[x, y] = (short)my;
                }
        }

        void Process2Backward(Raster input, Raster output, int inSize, int distance, int ss, short[,] distx, short[,] disty)
        {
            for (int y = inSize - 1; y >= 0; y--)
                for (int x = inSize - 1; x >= 0; x--)
                {
                    bool inner = input.Data[x, y] < 128;
                    var minDistance = inner ? int.MaxValue : int.MinValue;
                    int mx = distance;
                    int my = distance;

                    for (int k = 0; k < 5; k++)
                    {
                        int dx = 0;
                        int dy = 0;
                        switch (k)
                        {
                            case 0: dx = 1; dy = 0; break;
                            case 1: dx = 1; dy = 1; break;
                            case 2: dx = 0; dy = 1; break;
                            case 3: dx = -1; dy = 1; break;
                            case 4: dx = 0; dy = 0; break;
                        }

                        int rx = x + dx;
                        int ry = y + dy;

                        if (rx < 0 || rx >= inSize || ry < 0 || ry >= inSize)
                            continue;

                        bool inner2 = input.Data[rx, ry] < 128;

                        if (inner == inner2)
                        {
                            dx += distx[rx, ry];
                            dy += disty[rx, ry];
                        }

                        var ddd = dx * dx + dy * dy;
                        if (!inner)
                            ddd = -ddd;
                        if (inner && ddd < minDistance)
                        {
                            minDistance = ddd;
                            mx = dx;
                            my = dy;
                        }
                        if (!inner && ddd > minDistance)
                        {
                            minDistance = ddd;
                            mx = dx;
                            my = dy;
                        }
                    }
                    if (mx > short.MaxValue) mx = short.MaxValue;
                    if (mx < short.MinValue) mx = short.MinValue;
                    if (my > short.MaxValue) my = short.MaxValue;
                    if (my < short.MinValue) my = short.MinValue;
                    distx[x, y] = (short)mx;
                    disty[x, y] = (short)my;

                    if (x % ss == ss / 2 && y % ss == ss / 2)
                    {
                        bool minus = minDistance < 0;
                        double val = (Math.Sqrt(Math.Abs((double)minDistance)) / distance);
                        val = ((minus ? -val : val) + 1) / 2;
                        int v = (int)(val * 255);
                        if (v < 0)
                            v = 0;
                        if (v > 255)
                            v = 255;

                        var b = (byte)v;
                        var b1 = output.Data[x / ss, y / ss];

                        output.Data[x / ss, y / ss] = b;
                    }
                }
        }
    }
}
