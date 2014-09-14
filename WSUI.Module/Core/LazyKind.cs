using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Module.Interface;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.Core
{
    public class LazyKind : ILazyKind, INotifyPropertyChanged
    {
        private readonly Type _typeKind;
        private IKindItem _kindItem;
        private readonly IUnityContainer _container;
        private readonly Action<object> _choose;
        private readonly Action<string> _propertychanged;
        private readonly IMainViewModel _parent;
        private readonly object _lock = new object();

        public LazyKind(IUnityContainer container, Type typeKind, IMainViewModel parent, Action<object> choose, Action<string> propertychanged)
        {
            _container = container;
            _typeKind = typeKind;
            _parent = parent;
            _choose = choose;
            _propertychanged = propertychanged;
            ChooseCommand = new DelegateCommand(OnChoose, () => true);
        }

        #region Implementation of ILazyKind

        public IKindItem Kind
        {
            get
            {
                if (_kindItem == null)
                {
                    lock (_lock)
                    {
                        CreateKind();
                    }
                }
                return _kindItem;
            }
        }
        public string UIName { get; set; }
        public string Icon { get; private set; }
        public string Data { get; private set; }
        public int ID { get; protected set; }
        private bool _toggle;
        public bool Toggle
        {
            get { return _toggle; }
            set 
            { 
                _toggle = value; 
                OnPropertyChanged("Toggle"); 
            }
        }
        
        public void Initialize()
        {
            if (_typeKind == null)
            {
                UIName = "Default";
                return;
            }
            var customAttr = (KindNameIdAttribute[])_typeKind.GetCustomAttributes(typeof (KindNameIdAttribute), true);
            if (customAttr.Length == 0)
            {
                UIName = "Default";
                return;
            }
            UIName = customAttr[0].Name;
            ID = customAttr[0].Id;
            Icon = customAttr[0].Icon;
            Data = customAttr[0].Data;
            Toggle = false;
            switch (_parent.Host)
            {
                case HostType.Plugin:
                    CreateKind();
                    break;
            }
            OnPropertyChanged("UIName");
        }

        public ICommand ChooseCommand { get; protected set; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnChoose()
        {
            _choose(Kind);
        }

        private void CreateKind()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            _kindItem = (IKindItem)_container.Resolve(_typeKind);
            _kindItem.Init();
            _kindItem.Parent = _parent;
            if (_kindItem is INotifyPropertyChanged)
                (_kindItem as INotifyPropertyChanged).PropertyChanged += OnPropertyChanged;
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(String.Format("Resolve '{0}' takes {1}ms",_typeKind.FullName,watch.ElapsedMilliseconds));
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _propertychanged(propertyChangedEventArgs.PropertyName);
        }
    }
}
