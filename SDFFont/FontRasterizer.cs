using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SDFFont
{
    class FontRasterizer : IDisposable
    {
        private readonly RectangleF _rectf;
        private readonly Rectangle _rect;
        private readonly Bitmap _bitmap;
        private readonly byte[,] _data;
        private readonly byte[] _raw;
        private int _size;

        public FontRasterizer(int size)
        {
            _size = size;
            _bitmap = new Bitmap(size, size, PixelFormat.Format24bppRgb);
            _data = new byte[_size, _size];
            _raw = new byte[_size * _size * 3];
            _rectf = new RectangleF(0, 0, _size, _size);
            _rect = new Rectangle(0, 0, _size, _size);
        }

        public byte[,] Rasterize(string symbol)
        {
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            using (var g = Graphics.FromImage(_bitmap))
            {
                g.FillRectangle(Brushes.White, _rect);
                g.DrawString(symbol, new Font("Lucida Console", _size * 3 / 4), Brushes.Black, _rectf, sf);
            }

            var bits = _bitmap.LockBits(_rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(bits.Scan0, _raw, 0, _raw.Length);
            _bitmap.UnlockBits(bits);

            int k = 0;
            for (int y = 0; y < _size; y++)
                for (int x = 0; x < _size; x++)
                {
                    _data[x, y] = _raw[k];
                    k += 3;
                }

            return _data;
        }

        public void Dispose()
        {
            _bitmap.Dispose();
        }
    }
}
