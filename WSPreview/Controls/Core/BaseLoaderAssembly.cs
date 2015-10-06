using System;
using System.Reflection;
using OF.Core.Logger;

namespace OFPreview.Controls.Core
{
    public abstract class BaseLoaderAssembly
    {
        #region [const]

        protected const string Lib = @"\lib\";
        protected const string Lib64 = @"\lib64\";

        #endregion

        #region [fields]
        protected Assembly _assembly = null;

        #endregion


        #region[ctor]

        protected BaseLoaderAssembly()
        {
            
        }

        #endregion

        protected abstract string GetAssemblyName();

        protected virtual void Init()
        {
            _assembly = Environment.Is64BitProcess ? LoadAssembly(Lib64) : LoadAssembly(Lib);
        }

        private Assembly LoadAssembly(string folder)
        {
            string loc = Assembly.GetExecutingAssembly().Location;
            loc = loc.Substring(0, loc.LastIndexOf("\\"));
            loc = string.Format("{0}{1}{2}", loc, folder, GetAssemblyName());
            OFLogger.Instance.LogDebug(string.Format("Assembly path: {0}",loc));
            Assembly dll = Assembly.LoadFrom(loc);
            return dll;
        }

        

    }
}