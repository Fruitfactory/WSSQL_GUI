using System;
using System.Collections.Generic;
using System.Linq;

namespace OF.Module.Service
{
    public class OFFileExestensionsHelper
    {
        #region [needs]

        private List<string> _extensionCollection = new List<string>();

        #endregion

        #region [ctor]

        protected OFFileExestensionsHelper()
        {
            
        }

        #endregion

        #region [static]

        private static Lazy<OFFileExestensionsHelper> _instance = new Lazy<OFFileExestensionsHelper>(() =>
                                                                                                     {
                                                                                                         var inst =
                                                                                                             new OFFileExestensionsHelper
                                                                                                                 ();
                                                                                                         inst.Init();
                                                                                                         return inst;
                                                                                                     });

        public static OFFileExestensionsHelper Instance
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