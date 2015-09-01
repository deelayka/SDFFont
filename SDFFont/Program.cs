using System;
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

            var font = new FontRasterizer();
            var sdf = new SDF();

            var atlas = new Raster(outSize * 16);

            for (int cy = 0; cy < 16; cy++)
                for (int cx = 0; cx < 16; cx++)
                {
                    Console.WriteLine("{0}/256 complete.", (cx + 16 * cy));
                    string letter = Encoding.GetEncoding(1251).GetString(new byte[] { (byte)(cx + 16 * cy) });

                    var raster = font.Rasterize(letter, inSize);

                    var fragment = sdf.Process(raster, inSize, outSize, distance, ss);

                    atlas.Draw(fragment, cx * outSize, cy * outSize);

                }

            atlas.Save("D:\\out.png");
        }
    }
}
