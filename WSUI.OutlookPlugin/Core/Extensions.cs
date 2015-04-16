using System;
using System.Globalization;
using System.Reflection;

namespace OFOutlookPlugin.Core
{
    public static class Extensions
    {
        public static void HideForm(this AddinExpress.OL.ADXOlFormsManager manager)
        {
            Type t = manager.GetType();
            var m = t.GetMethod("HideAllForms", BindingFlags.NonPublic | BindingFlags.Instance);
            if (m != null)
            {
                m.Invoke(manager, BindingFlags.NonPublic, null, null, CultureInfo.CurrentUICulture);
            }            
        }

    }
}