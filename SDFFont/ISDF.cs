namespace SDFFont
{
    interface ISDF
    {
        void Process(Raster input, Raster output, int inSize, int outSize, int distance, int ss);
    }
}
