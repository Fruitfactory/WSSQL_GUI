using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using OF.Core.Interfaces;

namespace OF.Module.Interface.ViewModel
{
    public interface IContactKindDetailsViewModel<T> : IDisposable where T : ISearchObject
    {
        object View { get; }

        ObservableCollection<T> ItemSource { get; }

        ObservableCollection<T> PreviewSource { get; }
        string SearchString { get; set; }

        ISearchObject SelectedObject { get; set; }

        ISearchObject TrackedObject { get; set; }
        bool IsBusy { get; }
        bool IsMoreVisible { get; }
        double Height { get; }

        void Initialize();

        void SetSearchContext(string from, string to);

        void RunSearching();

        event EventHandler SearchFinished;

        ICommand HeightCalculateCommand { get; }

        ICommand KeySearchCommand { get; }

    }
}