using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SDFFont
{
    class Program
    {
        static void Main(string[] args)
        {
            int inSize = 4096;
            int outSize = 64;
            int distance = 512;

            int ss = inSize / outSize;

            var data = new byte[4096, 4096];

            RectangleF rectf = new RectangleF(0, 0, 4096, 4096);
            using (var bmp = new Bitmap(4096, 4096, PixelFormat.Format24bppRgb))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.FillRectangle(Brushes.White, 0, 0, 4096, 4096);
                    g.DrawString("A", new Font("Lucida Console", 3000), Brushes.Black, rectf);
                    g.Flush();
                }


                bmp.Save("D:\\f.png");

                for (int y = 0; y < 4096; y++)
                    for (int x = 0; x < 4096; x++)
                    {
                        var px = bmp.GetPixel(x, y);
                        data[x, y] = px.R;
                    }
            }

            var bmp2 = new Bitmap(outSize, outSize, PixelFormat.Format32bppArgb);
            for (int y = 0; y < outSize; y++)
                for (int x = 0; x < outSize; x++)
                {
                    bool inner = data[x * ss + ss / 2, y * ss + ss / 2] < 128;
                    var minDistance = inner ? int.MaxValue : int.MinValue;

                    for (int j = -distance; j <= distance; j++)
                        for (int i = -distance; i <= distance; i++)
                        {
                            var xi = x * ss + ss / 2 + i;
                            var yj = y * ss + ss / 2 + j;
                            if (xi < 0) xi = 0;
                            if (yj < 0) yj = 0;
                            if (xi > 4095) xi = 4095;
                            if (yj > 4095) yj = 4095;

                            bool isInner = data[xi, yj] < 128;

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
                    bmp2.SetPixel(x, y, System.Drawing.Color.FromArgb(255, v, v, v));
                }
            bmp2.Save("D:\\d.png");
        }
    }
}
