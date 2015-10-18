using System;

namespace SDFFont
{
    class SDFLinear : ISDF
    {
        private struct Point
        {
            public short X;
            public short Y;
        }

        private readonly Point[,] _dist;

        public SDFLinear(int inSize)
        {
            _dist = new Point[inSize, inSize];
        }

        public void Process(Raster input, Raster output, int inSize, int outSize, int distance, int ss)
        {
            ProcessForward(input, output, inSize, distance, ss, _dist);
            ProcessBackward(input, output, inSize, distance, ss, _dist);
        }

        void ProcessForward(Raster input, Raster output, int inSize, int distance, int ss, Point[,] dist)
        {
            for (int y = 0; y < inSize; y++)
                for (int x = 0; x < inSize; x++)
                {
                    bool inner = input.Data[y, x] < 128;
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

                        bool inner2 = input.Data[ry, rx] < 128;

                        if (inner == inner2)
                        {
                            dx += dist[ry, rx].X;
                            dy += dist[ry, rx].Y;
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
                    dist[y, x].X = (short)mx;
                    dist[y, x].Y = (short)my;
                }
        }

        void ProcessBackward(Raster input, Raster output, int inSize, int distance, int ss, Point[,] dist)
        {
            for (int y = inSize - 1; y >= 0; y--)
                for (int x = inSize - 1; x >= 0; x--)
                {
                    bool inner = input.Data[y, x] < 128;
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

                        bool inner2 = input.Data[ry, rx] < 128;

                        if (inner == inner2)
                        {
                            dx += dist[ry, rx].X;
                            dy += dist[ry, rx].Y;
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
                    dist[y, x].X = (short)mx;
                    dist[y, x].Y = (short)my;

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

                        output.Data[y / ss, x / ss] = (byte)v;
                    }
                }
        }
    }
}
