using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SDFFont
{
    class Raster
    {
        private readonly int _size;
        private readonly byte[,] _data;

        public Raster(int size)
        {
            _size = size;
            _data = new byte[size, size];
        }

        public int Size
        {
            get { return _size; }
        }

        public byte[,] Data
        {
            get { return _data; }
        }

        internal void Draw(Raster fragment, int x1, int y1)
        {
            for (int y = 0; y < fragment.Size; y++)
                for (int x = 0; x < fragment.Size; x++)
                    Data[y1 + y, x1 + x] = fragment.Data[y, x];
        }

        internal void Save(string filename)
        {
            var data = new byte[Size * Size * 4];

            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                {
                    data[0 + 4 * (x + y * Size)] = Data[y, x];
                    data[1 + 4 * (x + y * Size)] = Data[y, x];
                    data[2 + 4 * (x + y * Size)] = Data[y, x];
                    data[3 + 4 * (x + y * Size)] = 255;
                }

            using (var bitmap = new Bitmap(Size, Size, PixelFormat.Format32bppArgb))
            {
                var bits = bitmap.LockBits(new Rectangle(0, 0, Size, Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                Marshal.Copy(data, 0, bits.Scan0, data.Length);
                bitmap.UnlockBits(bits);
                bitmap.Save(filename);
            }
        }
    }
}
