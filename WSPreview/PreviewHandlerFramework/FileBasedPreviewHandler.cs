// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using OF.Core.Data;

namespace OFPreview.PreviewHandler.PreviewHandlerFramework
{
    public abstract class FileBasedPreviewHandler : PreviewHandler, IInitializeWithFile, IInitializeWithSearchObject
    {
        private string _filePath;
        private BaseSearchObject _searchObject;

        int IInitializeWithFile.Initialize(string pszFilePath, uint grfMode)
        {
            _filePath = pszFilePath;
            return 0;
        }

        protected override void Load(PreviewHandlerControl c)
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                c.Load(new FileInfo(_filePath));
                return;
            }
            if (_searchObject != null)
            {
                c.Load(_searchObject);
                return;
            }
        }

        public int Initialize(BaseSearchObject obj)
        {
            _searchObject = obj;
            return 0;
        }
    }
}
