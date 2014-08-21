using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using WSUI.Module.Core;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.ViewModel
{
    public class ContactDetailsViewModel : ViewModelBase,IContactDetailsViewModel
    {

        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        public ContactDetailsViewModel(IRegionManager regionManager,IEventAggregator eventAggregator,IContactDetailsView contactDetailsView)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            View = contactDetailsView;

        }


        public object View { get; private set; }


    }
}