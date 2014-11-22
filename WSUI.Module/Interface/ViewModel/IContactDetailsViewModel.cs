using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using WSUI.Core.Data;
using WSUI.Core.Data.UI;
using WSUI.Core.Interfaces;

namespace WSUI.Module.Interface.ViewModel
{
    public interface IContactDetailsViewModel : INotifyPropertyChanged
    {
        object View { get; }

        ObservableCollection<AttachmentSearchObject> ItemsSource { get; }

        ObservableCollection<EmailSearchObject> EmailsSource { get; } 

        void SetDataObject(ISearchObject dataSearchObject);

        bool IsSameData(ISearchObject dataObject);

        ISearchObject SelectedElement { get; set; }

        ISearchObject TrackedElement { get; set; }

        void ApplyIndexForShowing(int index);

        IEnumerable<UIItem> ContactUIItemCollection { get; }
        int SelectedIndex { get; }

        string SearchCriteria { get; }



    }
}