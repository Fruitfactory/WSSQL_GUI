using System;
using System.Windows;

namespace WSUI.Module.Service.Behaviors
{
    public class WSUIActualHeightBehavior
    {

        public static readonly DependencyProperty IsObserveProperty = DependencyProperty.RegisterAttached(
            "IsObserve", typeof (bool), typeof (WSUIActualHeightBehavior), new PropertyMetadata(default(bool), IsObserveChanged));

        private static void IsObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as FrameworkElement;
            if(element ==  null)
                return;
            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
            {
                element.SizeChanged += ElementOnSizeChanged;
            }
            else
            {
                element.SizeChanged -= ElementOnSizeChanged;
            }
        }

        private static void ElementOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var frameworkElement = sender as FrameworkElement;
            if(frameworkElement == null)
                return;
            frameworkElement.SetValue(WSUIActualHeightBehavior.ActualHeightProperty,frameworkElement.ActualHeight);
        }

        public static void SetIsObserve(DependencyObject element, bool value)
        {
            element.SetValue(IsObserveProperty, value);
        }

        public static bool GetIsObserve(DependencyObject element)
        {
            return (bool) element.GetValue(IsObserveProperty);
        }


        public static readonly DependencyProperty ActualHeightProperty = DependencyProperty.RegisterAttached(
            "ActualHeight", typeof (double), typeof (WSUIActualHeightBehavior), new PropertyMetadata(default(double)));


        public static void SetActualHeight(DependencyObject element, double value)
        {
            element.SetValue(ActualHeightProperty, value);
        }

        public static double GetActualHeight(DependencyObject element)
        {
            return (double) element.GetValue(ActualHeightProperty);
        } 
    }
}