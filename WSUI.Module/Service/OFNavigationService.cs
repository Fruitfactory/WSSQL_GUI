using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Transitionals;
using Transitionals.Transitions;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Infrastructure;
using OF.Module.Interface.Service;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Service
{
    internal class OFNavigationService : INavigationService
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

        public OFNavigationService(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _moveToLeftTransition = new TranslateTransition() { Duration = new Duration(TimeSpan.FromSeconds(0.15)), StartPoint = new Point(1.0, 0.0) };
            _moveToRightTransition = new TranslateTransition() { Duration = new Duration(TimeSpan.FromSeconds(0.15)) };
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

        public void MoveToLeft(INavigationView newView,bool useTransaction = true)
        {
            SetTransition(useTransaction ? _moveToLeftTransition : null);
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
                    System.Diagnostics.Debug.WriteLine("Old - " + oldView.GetType().Name);
                }
                var newView = _stackViews.Pop();
                regionSidebarData.Add(newView, newView.GetType().FullName);
                CurrentView = newView;
                System.Diagnostics.Debug.WriteLine("New - " + CurrentView.GetType().Name);

                CommonExtensions.ForEach(_stackViews, t =>
                {
                    System.Diagnostics.Debug.WriteLine("=> Left - " + t.GetType().Name);
                });
            }
        }

        public bool IsBackButtonVisible 
        {
            get { return _stackViews != null && _stackViews.Count > 0; }
        }

        public bool IsPreviewVisible { get { return CurrentView is IPreviewView; } }
        public IPreviewView PreviewView 
        {
            get { return CurrentView as IPreviewView; }
        }

        public bool IsContactDetailsVisible 
        {
            get { return CurrentView is IContactDetailsView; }
        }

        public IContactDetailsViewModel ContactDetailsViewModel
        {
            get
            {
                var contactDetailsViewModel = (CurrentView as IContactDetailsView);
                return contactDetailsViewModel != null ? contactDetailsViewModel.Model : null;
            }

        }


        public void MoveToFirstDataView(bool useTransaction = true)
        {
            if(CurrentView is IDataKindView)
                return;
            SetTransition(useTransaction ? _moveToRightTransition : null);
            IRegion regionSidebarData = _regionManager.Regions[RegionNames.SidebarDataRegion];
            if (regionSidebarData != null)
            {
                if (CurrentView != null)
                {
                    var oldView = regionSidebarData.GetView(CurrentView.GetType().FullName);
                    if (oldView != null)
                    {
                        regionSidebarData.Remove(oldView);
                        ReleaseContactDetailsViewModel(oldView as INavigationView);
                    }    
                }

                while (_stackViews.Count > 0) 
                {
                    var navigationView = _stackViews.Pop();
                    if (!(navigationView is IDataKindView))
                    {
                        ReleaseContactDetailsViewModel(navigationView);
                    }
                    else
                    {
                        regionSidebarData.Add(navigationView, navigationView.GetType().FullName);
                        CurrentView = navigationView;
                        break;
                    }
                }
            }
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