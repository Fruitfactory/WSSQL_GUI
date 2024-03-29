// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;

namespace OFPreview.PreviewHandler.PreviewHandlerFramework
{
    public abstract class FileBasedPreviewHandlerControl : PreviewHandlerControl
    {
        protected static FileInfo MakeTemporaryCopy(FileInfo file)
        {
            string tempPath = CreateTempPath(Path.GetExtension(file.Name));
            using (FileStream to = File.OpenWrite(tempPath))
            using (FileStream from = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                byte[] buffer = new byte[4096];
                int read;
                while ((read = from.Read(buffer, 0, buffer.Length)) > 0) to.Write(buffer, 0, read);
            }
            return new FileInfo(tempPath);
        }
    }
}
