using System;
using WSPreview.Controls.Core;

namespace WSPreview.PreviewHandler.TypeResolver
{
    public class PdfTypeLoader : BaseLoaderAssembly
    {
        private const string PdfWrapperTypeName = "PDFLibNet.PDFWrapper";
        private const string PdfAssemblyName = "PDFLibNet.dll";


        private Type _pdfWrapperType;

        #region [ctor]

        private PdfTypeLoader()
        {
            AssemblyName = PdfAssemblyName;
        }

        #endregion

        #region [static instance]

        private static Lazy<PdfTypeLoader> _instance = new Lazy<PdfTypeLoader>(() =>
                                                                               {
                                                                                   var inst = new PdfTypeLoader();
                                                                                   inst.Init();
                                                                                   return inst;
                                                                               });

        public static PdfTypeLoader Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        protected override void Init()
        {
            base.Init();
            if (_assembly != null)
            {
                _pdfWrapperType = _assembly.GetType(PdfWrapperTypeName);
            }
        }


        public Type GetPdfWrapperType()
        {
            return _pdfWrapperType;
        }

        public dynamic GetPdfWrapper()
        {
            return _pdfWrapperType != null
                ? Activator.CreateInstance(_pdfWrapperType)
                : null;
        }


    }
}