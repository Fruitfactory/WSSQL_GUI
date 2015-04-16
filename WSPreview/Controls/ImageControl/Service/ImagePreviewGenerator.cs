using System;
using System.Drawing;
using OFPreview.Controls.Core;
using OFPreview.PreviewHandler.Controls.ImageControl.Interface;

namespace OFPreview.PreviewHandler.Controls.ImageControl.Service
{
    public class ImagePreviewGenerator : BaseLoaderAssembly, IImagePreviewGenerator
    {
        #region [needs]

        #region [const]

        private const string TypeName = "GflWrapper.GflImageWrapper";
        private const string AssemblyWrapperName = "GflWrapper.dll";

        #endregion

        #region [fileds]

        private string _filename = string.Empty;
        private dynamic _gflInst = null;
        private Type _gfltype = null;

        #endregion

        #endregion

        #region [static]

        private static Lazy<IImagePreviewGenerator> _instance =  new Lazy<IImagePreviewGenerator>(() =>
                                                                                                      {
                                                                                                          var inst =
                                                                                                              new ImagePreviewGenerator
                                                                                                                  ();
                                                                                                          inst.Init();
                                                                                                          return inst;
                                                                                                      });
        public static IImagePreviewGenerator Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region [ctor]

        private ImagePreviewGenerator()
        {
            AssemblyName = AssemblyWrapperName;
        }

        #endregion

        #region Implementation of IImagePreviewGenerator

        public void SetFileName(string filename)
        {
            _filename = filename;
            _gflInst = Activator.CreateInstance(_gfltype, _filename);
        }

        public bool IsSupportFormat()
        {
            return _gflInst != null ? _gflInst.IsSupportExt() : false;
        }

        public Image GetImage()
        {
            return _gflInst != null ? _gflInst.GetImage() : null;
        }

        #endregion

        #region [private]

        protected override void Init()
        {
            base.Init();
            if (_assembly != null)
            {
                _gfltype = _assembly.GetType(TypeName);
            }
        }

        #endregion
        

    }
}