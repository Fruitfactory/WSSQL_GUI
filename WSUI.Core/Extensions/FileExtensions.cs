using System;
using System.IO;

namespace OF.Core.Extensions
{
    public static class FileExtensions
    {
        private static string[] VideoExtensions = new string[] { ".FLV", ".ASF", ".AVI", ".WMV", ".MP4", ".MOV", ".3GP", ".M4V", ".3G2", ".MPEG", ".MPG", ".MPE" };
        public static bool IsVideoFile(this string filename)
        {
            return Array.IndexOf(VideoExtensions, Path.GetExtension(filename).ToUpperInvariant()) > -1;
        }

    }
}