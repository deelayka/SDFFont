using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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

            var data = new byte[inSize, inSize];
            var raw = new byte[inSize * inSize * 3];

            var rectf = new RectangleF(0, 0, inSize, inSize);
            var rect = new Rectangle(0, 0, inSize, inSize);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            using (var bmp = new Bitmap(inSize, inSize, PixelFormat.Format24bppRgb))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.FillRectangle(Brushes.White, 0, 0, inSize, inSize);
                    g.DrawString("A", new Font("Lucida Console", inSize * 3 / 4), Brushes.Black, rectf, sf);
                }

                bmp.Save("D:\\f.png");

                var bits = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                Marshal.Copy(bits.Scan0, raw, 0, raw.Length);
                bmp.UnlockBits(bits);
            }

            int k = 0;
            for (int y = 0; y < inSize; y++)
                for (int x = 0; x < inSize; x++)
                {
                    data[x, y] = raw[k];
                    k += 3;
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
                            if (xi >= inSize) xi = inSize - 1;
                            if (yj >= inSize) yj = inSize - 1;

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
