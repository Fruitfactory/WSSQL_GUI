using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler
{
    public class HelperRegisterPreviewHandlers
    {

        public static void RegisterHandlers()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(PreviewHandler.PreviewHandlerFramework.PreviewHandler));
            var listTypes = assembly.GetTypes().Where(type => type.IsSealed && (type.IsSubclassOf(typeof(C4F.DevKit.PreviewHandler.PreviewHandlerFramework.FileBasedPreviewHandler)) || type.IsSubclassOf(typeof(C4F.DevKit.PreviewHandler.PreviewHandlerFramework.StreamBasedPreviewHandler)))).ToList();
            if (listTypes.Count > 0)
            {
                listTypes.ForEach(type =>
                    {
                        PreviewHandler.PreviewHandlerFramework.PreviewHandler.Register(type);
                    });
            }
        }

        public static void UnregisterHandlers()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(PreviewHandler.PreviewHandlerFramework.PreviewHandler));
            var listTypes = assembly.GetTypes().Where(type => type.IsSealed && (type.IsSubclassOf(typeof(C4F.DevKit.PreviewHandler.PreviewHandlerFramework.FileBasedPreviewHandler)) || type.IsSubclassOf(typeof(C4F.DevKit.PreviewHandler.PreviewHandlerFramework.StreamBasedPreviewHandler)))).ToList();
            if (listTypes.Count > 0)
            {
                listTypes.ForEach(type =>
                {
                    PreviewHandler.PreviewHandlerFramework.PreviewHandler.Unregister(type);
                });
            }
        }
    }
}
