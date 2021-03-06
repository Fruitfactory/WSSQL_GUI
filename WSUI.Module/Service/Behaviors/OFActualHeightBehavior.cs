﻿using System;
using System.Windows;

namespace OF.Module.Service.Behaviors
{
    public class OFActualHeightBehavior
    {

        public static readonly DependencyProperty IsObserveProperty = DependencyProperty.RegisterAttached(
            "IsObserve", typeof (bool), typeof (OFActualHeightBehavior), new PropertyMetadata(default(bool), IsObserveChanged));

        private static void IsObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as FrameworkElement;
            if(element ==  null)
                return;
            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
            {
                element.Initialized += ElementLoaded;
                element.SizeChanged += ElementOnSizeChanged;
            }
            else
            {
                element.Initialized -= ElementLoaded;
                element.SizeChanged -= ElementOnSizeChanged;
            }
        }

        private static void ElementLoaded(object sender,EventArgs args)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null)
                return;
            frameworkElement.SetValue(OFActualHeightBehavior.ActualHeightProperty, frameworkElement.ActualHeight);
        }

        private static void ElementOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var frameworkElement = sender as FrameworkElement;
            if(frameworkElement == null)
                return;
            frameworkElement.SetValue(OFActualHeightBehavior.ActualHeightProperty,frameworkElement.ActualHeight);
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
            "ActualHeight", typeof (double), typeof (OFActualHeightBehavior), new PropertyMetadata(default(double)));


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