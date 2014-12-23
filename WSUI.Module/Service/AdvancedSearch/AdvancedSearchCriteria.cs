using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Office.Interop.Outlook;
using WSUI.Infrastructure.Service.Dumper;
using WSUI.Module.Core;
using WSUI.Module.Enums;
using WSUI.Module.Interface.Service;

namespace WSUI.Module.Service.AdvancedSearch
{
    internal class AdvancedSearchCriteria : DataViewModel, IAdvancedSearchCriteria
    {


        public AdvancedSearchCriteria(ICommand addCommand, ICommand removeCommand)
        {
            AddCommand = addCommand;
            RemoveCommand = removeCommand;
            CriteriaTypes = Enum.GetValues(typeof(AdvancedSearchCriteriaType)).OfType<AdvancedSearchCriteriaType>().ToList();
        }

        public IEnumerable<AdvancedSearchCriteriaType> CriteriaTypes
        {
            get { return Get(() => CriteriaTypes); } 
            private set {Set(() =>  CriteriaTypes,value);}
        }

        public AdvancedSearchCriteriaType CriteriaType
        {
            get { return Get(() => CriteriaType); }
            set {Set(() => CriteriaType,value);}
        }

        public string Value
        {
            get { return Get(() => Value); }
            set { Set(() => Value, value); }
        }

        public bool RemoveButtonVisibility
        {
            get { return Get(() => RemoveButtonVisibility); }
            set { Set(() => RemoveButtonVisibility, value); }
        }

        public ICommand AddCommand
        {
            get { return Get(() => AddCommand); }
            private set { Set(() => AddCommand, value); }
        }

        public ICommand RemoveCommand
        {
            get { return Get(() => RemoveCommand); }
            private set { Set(() => RemoveCommand, value); }
        }

        #region [private]



        #endregion



    }
}