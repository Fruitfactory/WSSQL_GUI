using System;

namespace OF.Downloader.Interfaces
{
    public interface IMainView
    {
         object Model { get; set; }

        void ShowModal();

        event EventHandler Closed;
    }
}