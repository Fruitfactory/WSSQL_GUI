using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using OF.Infrastructure.Services;

namespace OF.Infrastructure.Behaviors
{
    public class OFEventToComandBehavior : Behavior<FrameworkElement>
    {
        #region [need]

        private Delegate _handler;
        private EventInfo _oldEvent;

        #endregion [need]

        #region [ctor]

        /// <summary>
        /// Initializes a new instance of the <see cref="ARMEventToCommandBehavior"/> class.
        /// </summary>
        public OFEventToComandBehavior()
        {
        }

        #endregion [ctor]

        #region [property]

        public static DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand),
                                                                                       typeof(OFEventToComandBehavior),
                                                                                       new PropertyMetadata(null));
        
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static DependencyProperty PassArgumentsProperty = DependencyProperty.Register("PassArguments", typeof(bool),
                                                                                             typeof(
                                                                                               OFEventToComandBehavior),
                                                                                             new PropertyMetadata(false));

        
        public bool PassArguments
        {
            get { return (bool)GetValue(PassArgumentsProperty); }
            set { SetValue(PassArgumentsProperty, value); }
        }

        public static DependencyProperty PassSenderProperty = DependencyProperty.Register("PassSender", typeof(bool),
                                                                                             typeof(
                                                                                               OFEventToComandBehavior),
                                                                                             new PropertyMetadata(false));

        
        public bool PassSender
        {
            get { return (bool)GetValue(PassSenderProperty); }
            set { SetValue(PassSenderProperty, value); }
        }

        public static DependencyProperty EventProperty = DependencyProperty.Register("Event", typeof(string),
                                                                                     typeof(OFEventToComandBehavior),
                                                                                     new PropertyMetadata(null,
                                                                                                          OnEventChanged));

        
        public string Event
        {
            get { return (string)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        
        public static void OnEventChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var beh = (OFEventToComandBehavior)o;
            if (beh != null && beh.AssociatedObject != null)
            {
                beh.AttachHandler((string)e.NewValue);
            }
        }

        #endregion [property]

        protected override void OnAttached()
        {
            this.AttachHandler(Event);
        }

        private void AttachHandler(string eventName)
        {
            if (_oldEvent != null)
            {
                _oldEvent.RemoveEventHandler(this.AssociatedObject, _handler);
            }
            if (!string.IsNullOrEmpty(Event))
            {
                EventInfo ei = this.AssociatedObject.GetType().GetEvent(eventName);
                if (ei != null)
                {
                    MethodInfo mi = this.GetType().GetMethod("ExecuteCommand", BindingFlags.Instance | BindingFlags.NonPublic);
                    _handler = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);
                    ei.AddEventHandler(this.AssociatedObject, _handler);
                    _oldEvent = ei;
                }
            }
        }

        private void ExecuteCommand(object sender, EventArgs e)
        {
            if (Command != null && Command.CanExecute(null))
            {
                if (PassSender)
                {
                    Command.Execute(sender);
                }
                else
                {
                    Command.Execute(PassArguments ? e : null);
                }
            }
        }
    }

}