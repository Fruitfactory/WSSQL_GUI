using System.Windows;

namespace OF.Downloader.Interfaces
{
    internal interface IMainViewModel
    {
        void Initialize();
        void Show();
        IMainView View { get; }
        void Dispose();
    }
}