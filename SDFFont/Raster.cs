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
    }
}
