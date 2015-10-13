using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace OF.Core.Core.AdvancedSearchCriteria
{
    public abstract class VariantAdvancedSearchCriteria<T> : OFAdvancedSearchCriteria where T : struct, IConvertible
    {
        protected VariantAdvancedSearchCriteria(ICommand addCommand, ICommand removeCommand) : base(addCommand, removeCommand)
        {
            InitVariants();
        }

        public IEnumerable<T> VariantSource
        {
            get { return Get(() => VariantSource); }
            set { Set(() => VariantSource,value);}
        }


        private void InitVariants()
        {
            if (!typeof (T).IsEnum)
            {
                VariantSource  = null;
                return;
            }
            VariantSource = Enum.GetValues(typeof(T)).OfType<T>().ToList();
        }


    }
}