using System;
using OF.Core.Enums;

namespace OF.Module.Interface.ViewModel
{
    public interface IOFEmailSuggestViewModel
    {
        void Show(Tuple<IntPtr,string> hWndParent);
        void Hide();
        
        void ProcessSelection(OFActionType type);

        void UpdateSuggectingList();
        IMainViewModel MainViewModel { get; set; }
    }
}