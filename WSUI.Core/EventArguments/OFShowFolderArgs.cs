namespace OF.Core.EventArguments
{
    public class OFShowFolderArgs : System.EventArgs
    {
        public OFShowFolderArgs(string folder)
        {
            Folder = folder;
        }

        public string Folder { get; private set; }
    }
}