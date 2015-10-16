using System;
using System.Diagnostics;
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

            var stopwatch = new Stopwatch();

            using (var font = new FontRasterizer(inSize))
            {
                ISDF sdf = new SDFLinear(inSize);
                var raster = new Raster(inSize);
                var fragment = new Raster(outSize);

                var atlas = new Raster(outSize * 16);

                for (int cy = 0; cy < 16; cy++)
                    for (int cx = 0; cx < 16; cx++)
                    {
                        stopwatch.Restart();
                        Console.Write("Step {0}/256", (cx + 16 * cy));

                        string letter = Encoding.GetEncoding(1251).GetString(new byte[] { (byte)(cx + 16 * cy) });

                        font.Rasterize(raster, letter);

                        sdf.Process(raster, fragment, inSize, outSize, distance, ss);

                        atlas.Draw(fragment, cx * outSize, cy * outSize);

                        Console.WriteLine(" completed in {0} ms.", stopwatch.ElapsedMilliseconds);
                        stopwatch.Stop();
                    }

                atlas.Save("D:\\out.png");
            }
        }
    }
}
