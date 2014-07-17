namespace WSUI.Core.EventArguments
{
    public class WSUIShowFolderArgs :  System.EventArgs
    {
        public WSUIShowFolderArgs(string folder)
        {
            Folder = folder;
        }

        public string Folder { get; private set; }
    }
}