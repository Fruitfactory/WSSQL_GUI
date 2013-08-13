using System;
using System.Collections.Generic;
using System.Linq;

namespace WSUI.Module.Service
{
    public class FileExestensionsHelper
    {
        #region [needs]

        private List<string> _extensionCollection = new List<string>();

        #endregion

        #region [ctor]

        protected FileExestensionsHelper()
        {
            
        }

        #endregion

        #region [static]

        private static Lazy<FileExestensionsHelper> _instance = new Lazy<FileExestensionsHelper>(() =>
                                                                                                     {
                                                                                                         var inst =
                                                                                                             new FileExestensionsHelper
                                                                                                                 ();
                                                                                                         inst.Init();
                                                                                                         return inst;
                                                                                                     });

        public static FileExestensionsHelper Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region [init]

        protected void Init()
        {
            FillExtensionsRequiredClosingPreview();
        }

        private void FillExtensionsRequiredClosingPreview()
        {
            _extensionCollection.Add("pdf");
        }

        #endregion

        #region [public]

        public bool IsExternsionRequiredClosePreview(string ext)
        {
            return _extensionCollection.Any(e => ext.ToLower().IndexOf(e, System.StringComparison.Ordinal) > -1);
        }

        #endregion

    }
}