using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SDFFont
{
    class FontRasterizer
    {
        public Raster Rasterize(string symbol, int size)
        {
            Raster raster = new Raster(size);

            using (var bitmap = new Bitmap(size, size, PixelFormat.Format24bppRgb))
            {
                var raw = new byte[size * size * 3];
                var rectf = new RectangleF(0, 0, size, size);
                var rect = new Rectangle(0, 0, size, size);

                var sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;

                using (var g = Graphics.FromImage(bitmap))
                {
                    g.FillRectangle(Brushes.White, rect);
                    g.DrawString(symbol, new Font("Lucida Console", size * 3 / 4), Brushes.Black, rectf, sf);
                }

                var bits = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                Marshal.Copy(bits.Scan0, raw, 0, raw.Length);
                bitmap.UnlockBits(bits);

                int k = 0;
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        raster.Data[x, y] = raw[k];
                        k += 3;
                    }
            }

            return raster;
        }
    }
}
