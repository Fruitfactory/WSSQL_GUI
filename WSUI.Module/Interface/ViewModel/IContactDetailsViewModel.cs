using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using OF.Core.Data;
using OF.Core.Data.UI;
using OF.Core.Interfaces;

namespace OF.Module.Interface.ViewModel
{
    public interface IContactDetailsViewModel : INotifyPropertyChanged
    {
        object View { get; }

        ObservableCollection<OFAttachmentContentSearchObject> ItemsSource { get; }

        ObservableCollection<OFEmailSearchObject> EmailsSource { get; } 

        void SetDataObject(ISearchObject dataSearchObject);

        bool IsSameData(ISearchObject dataObject);

        ISearchObject SelectedElement { get; set; }

        ISearchObject TrackedElement { get; set; }

        void ApplyIndexForShowing(int index);

        IEnumerable<OFUIItem> ContactUIItemCollection { get; }
        int SelectedIndex { get; }

        string SearchCriteria { get; }



    }
}