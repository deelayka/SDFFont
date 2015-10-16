using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SDFFont
{
    class FontRasterizer : IDisposable
    {
        private Bitmap _bitmap;
        private int _size;
        private byte[] _raw;

        public FontRasterizer(int size)
        {
            _size = size;
            _bitmap = new Bitmap(_size, _size, PixelFormat.Format24bppRgb);
            _raw = new byte[_size * _size * 3];
        }

        public void Rasterize(Raster raster, string symbol)
        {
            var rectf = new RectangleF(0, 0, _size, _size);
            var rect = new Rectangle(0, 0, _size, _size);

            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            using (var g = Graphics.FromImage(_bitmap))
            {
                g.FillRectangle(Brushes.White, rect);
                g.DrawString(symbol, new Font("Lucida Console", _size * 3 / 4), Brushes.Black, rectf, sf);
            }

            var bits = _bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(bits.Scan0, _raw, 0, _raw.Length);
            _bitmap.UnlockBits(bits);

            int k = 0;
            for (int y = 0; y < _size; y++)
                for (int x = 0; x < _size; x++)
                {
                    raster.Data[y, x] = _raw[k];
                    k += 3;
                }
        }

        public void Dispose()
        {
            _bitmap.Dispose();
        }
    }
}
