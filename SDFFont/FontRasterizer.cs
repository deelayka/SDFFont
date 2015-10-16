using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SDFFont
{
    class FontRasterizer : IDisposable
    {
        private readonly Bitmap _bitmap;
        private readonly int _size;
        private readonly byte[] _raw;

        private readonly Font _font;
        private readonly RectangleF _rectangleF;
        private readonly Rectangle _rectangle;
        private readonly StringFormat _format;

        public FontRasterizer(int size)
        {
            _size = size;
            _bitmap = new Bitmap(_size, _size, PixelFormat.Format24bppRgb);
            _raw = new byte[_size * _size * 3];

            _font = new Font("Lucida Console", _size * 3 / 4);
            _rectangleF = new RectangleF(0, 0, _size, _size);
            _rectangle = new Rectangle(0, 0, _size, _size);
            _format = new StringFormat();
            _format.Alignment = StringAlignment.Center;
        }

        public void Rasterize(Raster raster, string symbol)
        {
            using (var g = Graphics.FromImage(_bitmap))
            {
                g.FillRectangle(Brushes.White, _rectangle);
                g.DrawString(symbol, _font, Brushes.Black, _rectangleF, _format);
            }

            var bits = _bitmap.LockBits(_rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
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
