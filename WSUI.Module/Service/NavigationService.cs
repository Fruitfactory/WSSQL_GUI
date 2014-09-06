using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Transitionals;
using Transitionals.Transitions;
using WSUI.Core.Interfaces;
using WSUI.Infrastructure;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.Service
{
    internal class NavigationService : INavigationService
    {
        #region [const]

        private const string InternalDataView = "DataView";
        private const string InternalPreviewView = "PreviewView";
        private const string InternalSettingsView = "SettingsView";

        #endregion [const]

        #region [needs]

        private IRegionManager _regionManager;
        private Stack<INavigationView> _stackViews;
        private IMainViewModel _mainViewModel;

        private Transition _moveToLeftTransition = null;
        private Transition _moveToRightTransition = null;

        #endregion [needs]

        public NavigationService(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _moveToLeftTransition = new TranslateTransition() { Duration = new Duration(TimeSpan.FromSeconds(0.22)), StartPoint = new Point(1.0, 0.0) };
            _moveToRightTransition = new TranslateTransition() { Duration = new Duration(TimeSpan.FromSeconds(0.22)) };
            _stackViews = new Stack<INavigationView>();
        }

        #region [public]

        public INavigationView CurrentView { get; private set; }

        public void SetMainViewModel(IMainViewModel main)
        {
            _mainViewModel = main;
        }

        public void ShowSelectedKind(object kindItem)
        {
            IRegion regionSidebarSearch = _regionManager.Regions[RegionNames.SidebarSearchRegion];
            if (regionSidebarSearch != null)
            {
                object viewSettings = GetView(kindItem, InternalSettingsView);
                if (viewSettings != null && !regionSidebarSearch.Views.Contains(viewSettings))
                {
                    regionSidebarSearch.Add(viewSettings);
                    regionSidebarSearch.Activate(viewSettings);
                }
                else if (viewSettings != null)
                {
                    regionSidebarSearch.Activate(viewSettings);
                }
            }
            SetTransition(_moveToLeftTransition);
            IRegion regionSidebarData = _regionManager.Regions[RegionNames.SidebarDataRegion];
            if (regionSidebarData != null)
            {
                object viewData = GetView(kindItem, InternalDataView);
                var oldView = regionSidebarData.Views.FirstOrDefault();
                if (oldView != null)
                {
                    regionSidebarData.Remove(oldView);
                }
                regionSidebarData.Add(viewData, viewData.GetType().FullName);
                CurrentView = viewData as INavigationView;
            }
            _stackViews.Clear();
        }

        public void MoveToLeft(INavigationView newView)
        {
            SetTransition(_moveToLeftTransition);
            IRegion regionSidebarData = _regionManager.Regions[RegionNames.SidebarDataRegion];
            if (regionSidebarData != null)
            {
                var oldView = regionSidebarData.GetView(CurrentView.GetType().FullName);
                if (oldView != null)
                {
                    regionSidebarData.Remove(oldView);
                    _stackViews.Push(CurrentView);
                }
                regionSidebarData.Add(newView, newView.GetType().FullName);
                CurrentView = newView;
            }
        }

        public void MoveToRight()
        {
            SetTransition(_moveToRightTransition);
            IRegion regionSidebarData = _regionManager.Regions[RegionNames.SidebarDataRegion];
            if (regionSidebarData != null)
            {
                var oldView = regionSidebarData.GetView(CurrentView.GetType().FullName);
                if (oldView != null)
                {
                    regionSidebarData.Remove(oldView);
                    ReleaseContactDetailsViewModel(oldView as INavigationView);
                }
                var newView = _stackViews.Pop();
                regionSidebarData.Add(newView, newView.GetType().FullName);
                CurrentView = newView;
            }
        }

        public bool IsBackButtonVisible 
        {
            get { return _stackViews != null && _stackViews.Count > 0; }
        }

        #endregion [public]

        #region [private helpers]

        private object GetView(object viewModel, string propertyName)
        {
            PropertyInfo prop = viewModel.GetType().GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance);
            if (prop != null)
            {
                return prop.GetValue(viewModel, null);
            }
            return null;
        }

        private void SetTransition(Transition transition)
        {
            _mainViewModel.CurrenTransition = transition;
        }

        private void ReleaseContactDetailsViewModel(INavigationView view)
        {
            if (view == null)
                return;

            var contactDetailsView = view as IContactDetailsView;
            if (contactDetailsView == null)
                return;

            var disposable = contactDetailsView.Model as IDisposable;
            if (disposable == null)
                return;

            disposable.Dispose();
        }

        #endregion [private helpers]
    }
}