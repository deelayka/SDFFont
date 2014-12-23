using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

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

            using (var outBmp = new Bitmap(outSize * 16, outSize * 16, PixelFormat.Format32bppArgb))
            {
                using (var gfx = Graphics.FromImage(outBmp))

                    for (int cy = 0; cy < 16; cy++)
                        for (int cx = 0; cx < 16; cx++)
                        {
                            Console.WriteLine("{0}/256 complete.", (cx + 16 * cy));
                            string letter = Encoding.GetEncoding(1251).GetString(new byte[] { (byte)(cx + 16 * cy) });

                            var rectf = new RectangleF(0, 0, inSize, inSize);
                            var rect = new Rectangle(0, 0, inSize, inSize);
                            var sf = new StringFormat();
                            sf.Alignment = StringAlignment.Center;

                            using (var bmp = new Bitmap(inSize, inSize, PixelFormat.Format24bppRgb))
                            {
                                using (var g = Graphics.FromImage(bmp))
                                {
                                    g.FillRectangle(Brushes.White, 0, 0, inSize, inSize);
                                    g.DrawString(letter, new Font("Lucida Console", inSize * 3 / 4), Brushes.Black, rectf, sf);
                                }

                                //bmp.Save("D:\\f.png");

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

                            using (var bmp2 = new Bitmap(outSize, outSize, PixelFormat.Format32bppArgb))
                            {
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
                                                if (xi < 0 || yj < 0 || xi >= inSize || yj >= inSize)
                                                    continue;

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

                                //bmp2.Save("D:\\d.png");
                                gfx.DrawImageUnscaled(bmp2, new Point(cx * outSize, cy * outSize));
                            }
                        }
                outBmp.Save("D:\\out.png");
            }
        }
    }
}
