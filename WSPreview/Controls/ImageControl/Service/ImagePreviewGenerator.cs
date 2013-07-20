using System;
using System.Drawing;
using System.Reflection;
using WSPreview.PreviewHandler.Controls.ImageControl.Interface;

namespace WSPreview.PreviewHandler.Controls.ImageControl.Service
{
    public class ImagePreviewGenerator : IImagePreviewGenerator
    {
        #region [needs]

        #region [const]

        private const string TypeName = "GflWrapper.GflImageWrapper";
        private const string AssemblyWrapperName = "GflWrapper.dll";
        private const string Lib = @"\lib\";
        private const string Lib64 = @"\lib64\";

        #endregion

        #region [fileds]

        private string _filename = string.Empty;
        private dynamic _gflInst = null;
        private Assembly _assembly = null;
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

        }

        #endregion

        #region Implementation of IImagePreviewGenerator

        public void SetFileName(string filename)
        {
            _filename = filename;
            _gflInst = Activator.CreateInstance(_gfltype, _filename);
        }

        public bool IsSupportFofrmat()
        {
            return _gflInst != null ? _gflInst.IsSupportExt() : false;
        }

        public Image GetImage()
        {
            return _gflInst != null ? _gflInst.GetImage() : null;
        }

        #endregion

        #region [private]

        private void Init()
        {
            _assembly = Environment.Is64BitProcess ? LoadGflWrapperAssembly(Lib64) : LoadGflWrapperAssembly(Lib);
            if (_assembly != null)
            {
                _gfltype = _assembly.GetType(TypeName);
            }
        }

        private Assembly LoadGflWrapperAssembly(string folder)
        {
            string loc = Assembly.GetExecutingAssembly().Location;
            loc = loc.Substring(0, loc.LastIndexOf("\\"));
            loc = string.Format("{0}{1}{2}", loc, folder, AssemblyWrapperName);
            Assembly dll = Assembly.LoadFrom(loc);
            return dll;
        }
           

        #endregion
        

    }
}