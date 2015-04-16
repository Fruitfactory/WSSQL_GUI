namespace OF.Core.Helpers.DetectEncoding.Multilang
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct _LARGE_INTEGER
    {
        public long QuadPart;
    }
}