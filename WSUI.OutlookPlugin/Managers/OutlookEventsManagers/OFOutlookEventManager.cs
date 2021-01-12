using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Prism.Events;
using OF.Infrastructure.Events;
using OFOutlookPlugin.Events;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OFOutlookPlugin.Managers.OutlookEventsManagers
{
    public interface IOFOutlookEventManager : IDisposable
    {
        void SubscribeEvents();
        void UnsubscribeEvents();
    }

    public class OFOutlookEventManager : IOFOutlookEventManager
    {
        private Outlook.Application _application;
        private IEventAggregator _eventAggregator;
        
        private Dictionary<int,Outlook.Inspector> _inspectors = new Dictionary<int, Outlook.Inspector>();
        private Dictionary<int, Outlook.InspectorEvents_10_ActivateEventHandler> _activateEventHandlers = new Dictionary<int, Outlook.InspectorEvents_10_ActivateEventHandler>();
        private Dictionary<int, Outlook.InspectorEvents_10_CloseEventHandler> _closeEventHandlers = new Dictionary<int, Outlook.InspectorEvents_10_CloseEventHandler>();

        private Dictionary<int,Outlook.Explorer> _explorers = new Dictionary<int, Outlook.Explorer>();
        private Dictionary<int, Outlook.ExplorerEvents_10_ActivateEventHandler> _activateExplorer = new Dictionary<int, Outlook.ExplorerEvents_10_ActivateEventHandler>();
        private Dictionary<int, Outlook.ExplorerEvents_10_CloseEventHandler> _closeExplorer = new Dictionary<int, Outlook.ExplorerEvents_10_CloseEventHandler>();
        private Dictionary<int, Outlook.ExplorerEvents_10_SelectionChangeEventHandler> _selectionChangedExplorer = new Dictionary<int, Outlook.ExplorerEvents_10_SelectionChangeEventHandler>();
        private Dictionary<int,Outlook.ExplorerEvents_10_FolderSwitchEventHandler> _folderSwitchExplorer = new Dictionary<int, Outlook.ExplorerEvents_10_FolderSwitchEventHandler>();
        private Dictionary<int,  Outlook.ExplorerEvents_10_BeforeFolderSwitchEventHandler> _beforeFolderSwitchExplorer =  new Dictionary<int, Outlook.ExplorerEvents_10_BeforeFolderSwitchEventHandler>();


        private Outlook.Inspectors _origInspectors;
        private Outlook.Explorers _origExplorers;

        public OFOutlookEventManager(Outlook.Application application, IEventAggregator eventAggregator)
        {
            _application = application;
            _eventAggregator = eventAggregator;
        }


        public void SubscribeEvents()
        {
            ((Outlook.ApplicationEvents_11_Event)_application).Quit += ApplicationQuit;
            ((Outlook.ApplicationEvents_11_Event)_application).ItemSend += OnItemSend;

            _origInspectors = _application.Inspectors;
            _origExplorers = _application.Explorers;
            _origInspectors.NewInspector += InspectorsOnNewInspector;
            _origExplorers.NewExplorer += ExplorersOnNewExplorer;
        }

        public void UnsubscribeEvents()
        {
            Dispose();
        }

        private void ReleaseUnmanagedResources()
        {
            ((Outlook.ApplicationEvents_11_Event)_application).Quit -= ApplicationQuit;
            ((Outlook.ApplicationEvents_11_Event)_application).ItemSend -= OnItemSend;
            _application.Inspectors.NewInspector -= InspectorsOnNewInspector;
            _application.Explorers.NewExplorer -= ExplorersOnNewExplorer;
            if (_inspectors.Count > 0)
            {
                foreach (var keyValuePair in _inspectors)
                {
                    UnsubscribeInspector(keyValuePair.Value);
                }
                _inspectors.Clear();
            }
            if (_explorers.Count > 0)
            {
                foreach (var keyValuePair in _explorers)
                {
                    UnsubscribeExplorer(keyValuePair.Value);   
                }
                _explorers.Clear();
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~OFOutlookEventManager()
        {
            ReleaseUnmanagedResources();
        }

        private void ApplicationQuit()
        {
            _eventAggregator?.GetEvent<OFQuitEvent>().Publish(true);
        }

        private void InspectorsOnNewInspector(Outlook.Inspector inspector)
        {
            _eventAggregator?.GetEvent<OFNewInspectorEvent>().Publish(inspector);
            _inspectors.Add(inspector.GetHashCode(),inspector);

            Outlook.InspectorEvents_10_ActivateEventHandler ad = () => InspectorActivated(inspector);
            Outlook.InspectorEvents_10_CloseEventHandler cd = () => InspectorClosed(inspector);
            ((Outlook.InspectorEvents_10_Event)inspector).Activate += ad;
            ((Outlook.InspectorEvents_10_Event)inspector).Close += cd;
            _activateEventHandlers.Add(inspector.GetHashCode(),ad);
            _closeEventHandlers.Add(inspector.GetHashCode(),cd);
        }

        private void InspectorClosed(Outlook.Inspector inspector)
        {
            _eventAggregator?.GetEvent<OFActivatedInspectorEvent>().Publish(inspector);

            int hash = inspector.GetHashCode();

            if (_inspectors.ContainsKey(hash))
            {
                UnsubscribeInspector(inspector);
            }

        }

        private void InspectorActivated(Outlook.Inspector inspector)
        {
            _eventAggregator?.GetEvent<OFClosedInspectorEvent>().Publish(inspector);
        }

        private void UnsubscribeInspector(Outlook.Inspector inspector)
        {
            ((Outlook.InspectorEvents_10_Event)inspector).Activate -= _activateEventHandlers[inspector.GetHashCode()];
            ((Outlook.InspectorEvents_10_Event)inspector).Close -= _closeEventHandlers[inspector.GetHashCode()];
        }

        private void OnItemSend(object item, ref bool cancel)
        {
            _eventAggregator?.GetEvent<OFItemSendEvent>().Publish(item);
        }

        private void ExplorersOnNewExplorer(Outlook.Explorer explorer)
        {
            var hashCode = explorer.GetHashCode();

            _eventAggregator?.GetEvent<OFNewExplorerEvent>().Publish(explorer);
            _explorers.Add(hashCode, explorer);

            Outlook.ExplorerEvents_10_ActivateEventHandler ad = () => ActivatedExplorer(explorer);
            Outlook.ExplorerEvents_10_CloseEventHandler cd = () => ClosedExplorer(explorer);
            Outlook.ExplorerEvents_10_SelectionChangeEventHandler sd = () => ExplorerOnSelectionChange(explorer);
            Outlook.ExplorerEvents_10_FolderSwitchEventHandler fw = () => FolderSwitch(explorer);
            Outlook.ExplorerEvents_10_BeforeFolderSwitchEventHandler bfw = (object folder, ref bool cancel) => BeforeFolderSwitched(explorer,folder, ref cancel);
            _activateExplorer.Add(hashCode, ad);
            _closeExplorer.Add(hashCode, cd);
            _selectionChangedExplorer.Add(hashCode, sd);
            _folderSwitchExplorer.Add(hashCode, fw);
            _beforeFolderSwitchExplorer.Add(hashCode, bfw);
            ((Outlook.ExplorerEvents_10_Event) explorer).Activate += ad;
            ((Outlook.ExplorerEvents_10_Event)explorer).Close += cd;
            ((Outlook.ExplorerEvents_10_Event)explorer).SelectionChange += sd;
            explorer.FolderSwitch += fw;
            explorer.BeforeFolderSwitch += bfw;

        }

        private void ClosedExplorer(Outlook.Explorer explorer)
        {
            _eventAggregator?.GetEvent<OFClosedExplorerEvent>().Publish(explorer);

            if (_explorers.ContainsKey(explorer.GetHashCode()))
            {
                UnsubscribeExplorer(explorer);
            }
        }

        private void ActivatedExplorer(Outlook.Explorer explorer)
        {
            _eventAggregator?.GetEvent<OFActivatedExplorerEvent>().Publish(explorer);
        }

        private void UnsubscribeExplorer(Outlook.Explorer explorer)
        {
            int hash = explorer.GetHashCode();
            ((Outlook.ExplorerEvents_10_Event)explorer).Activate -= _activateExplorer[hash];
            ((Outlook.ExplorerEvents_10_Event)explorer).Close -= _closeExplorer[hash];
            ((Outlook.ExplorerEvents_10_Event)explorer).SelectionChange -= _selectionChangedExplorer[hash];
            explorer.FolderSwitch -= _folderSwitchExplorer[hash];
            explorer.BeforeFolderSwitch -= _beforeFolderSwitchExplorer[hash];
        }

        private void ExplorerOnSelectionChange(Outlook.Explorer explorer)
        {
            _eventAggregator?.GetEvent<OFSelectionChangedExplorerEvent>().Publish(explorer);
        }

        private void FolderSwitch(Outlook.Explorer explorer)
        {
            _eventAggregator?.GetEvent<OFFolderSwitchedEvent>().Publish(explorer);
        }

        private void BeforeFolderSwitched(Outlook.Explorer explorer, object folder, ref bool cancel)
        {
            var payload = new OFBeforeFolderSwitchedPayload(explorer,folder,cancel);
            _eventAggregator?.GetEvent<OFBeforeFolderSwitchedEvent>().Publish(payload);
            cancel = payload.Cancel;
        }

    }
}