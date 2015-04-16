using System;
using System.Collections.Specialized;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Transitionals.Controls;

namespace OF.Module.Service
{
    public class TransitionElementAdaptor : RegionAdapterBase<TransitionElement>
    {
        public TransitionElementAdaptor(IRegionBehaviorFactory behaviorFactory) :
            base(behaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, TransitionElement regionTarget)
        {
            region.Views.CollectionChanged += (s, e) =>
            {
                Transition(regionTarget, e);
            };

            region.ActiveViews.CollectionChanged += (s, e) =>
            {
                Transition(regionTarget, e);
            };
        }

        private void Transition(TransitionElement regionTarget, NotifyCollectionChangedEventArgs e)
        {
            //Add
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                foreach (FrameworkElement element in e.NewItems)
                    regionTarget.Content = element;

            //Removal
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                foreach (FrameworkElement element in e.OldItems)
                {
                    regionTarget.Content = null;
                    GC.Collect();
                }
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}