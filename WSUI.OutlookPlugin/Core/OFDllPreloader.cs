using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using AddinExpress.OL;
using OF.Core.Logger;

namespace OFOutlookPlugin.Core {
public class OFDllPreloader {
  #region [needs]

  private const string DllExt = "*.dll";
  private List<string> _listPartNameOfDll = new List<string>();

  #endregion

  #region [ctor]

  protected OFDllPreloader()
  {

  }
  #endregion

  #region [static]

  private static Lazy<OFDllPreloader> _instance = new Lazy<OFDllPreloader>(() =>
  {
    var inst = new OFDllPreloader();
    inst.Init();
    return inst;
  });

  public static OFDllPreloader Instance
  {
    get {
      return _instance.Value;
    }
  }

  #endregion

  #region [public]

  public void PreloadDll()
  {
    var preLoadThread = new Thread(DoPreInitialize);
    preLoadThread.Start(null);
    //PreloadDllInThread(null);
  }

  #endregion

  #region [private]

  private void Init()
  {
    // add part of dll name which should be loaded during loading plugin
    _listPartNameOfDll.Add("MahApps");
    _listPartNameOfDll.Add("Practices");
    _listPartNameOfDll.Add("OF");
    _listPartNameOfDll.Add("OFPreview");
  }

  private void DoPreInitialize(object obj )
  {
    PreloadDllInThread(obj);
  }

  private void PreloadDllInThread(object state)
  {
    var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
    string currentFolder = AppDomain.CurrentDomain.BaseDirectory;
    var files = Directory.GetFiles(currentFolder, DllExt, SearchOption.TopDirectoryOnly).Where(filename => _listPartNameOfDll.Any(s => Path.GetFileName(filename).Contains(s)));//.Where(filename => _listPartNameOfDll.Any(s => Path.GetFileName(filename).Contains(s)));
    if (!files.Any())
      return;
    foreach (var file in files)
    {
      try
      {
        var assembleName = AssemblyName.GetAssemblyName(file);
        if ( !assemblies.Any(a => AssemblyName.ReferenceMatchesDefinition(assembleName, a.GetName())))
        {
          OFLogger.Instance.LogDebug(string.Format("Preload: {0}",file));
          Assembly.LoadFrom(file);
        }
      }
      catch (Exception ex)
      {
        OFLogger.Instance.LogError(string.Format("Preload: {0}",ex.Message));
      }

    }
  }

  #endregion

}
}