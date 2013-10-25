using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;
using WSUI.Core.Extensions;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class GeneralContactRule : BaseSearchRule<BaseSearchObject>
    {
        #region [needs]

        private readonly IList<ISearch> _listContactsRules = new List<ISearch>(); 
        
        #endregion

        public GeneralContactRule()
        {
            ConstructorInit();
        }

        public GeneralContactRule(object lockObject)
            :base(lockObject)
        {
            ConstructorInit();
        }

        private void ConstructorInit()
        {
            Priority = 0;
            _listContactsRules.Add(new ContactSearchRule(Lock));
            _listContactsRules.Add(new EmailContactSearchRule(Lock));
        }

        public override void SetSearchCriteria(string criteria)
        {
            base.SetSearchCriteria(criteria);
            _listContactsRules.ForEach(r => r.SetSearchCriteria(Query));
        }

        public override void Stop()
        {
            base.Stop();
            _listContactsRules.ForEach(r => r.Stop());
        }

        protected override void DoQuery()
        {
            try
            {
                WaitHandle[] arrayEvents = _listContactsRules.Select(r => r.GetEvent()).ToArray();
                if ( arrayEvents ==  null || arrayEvents.Length == 0)
                    throw new ArgumentException("Array of events is empty");
                _listContactsRules.ForEach(r => r.Search());
                WaitHandle.WaitAll(arrayEvents);
                ProcessResult();
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("GeneralContact: {0}", ex.Message);
            }
            finally
            {
                Event.Set();
            }
        }

        protected override void ProcessResult()
        {
            var resultEmailContact = (_listContactsRules[1] as ISearchRule).GetResults().OperationResult.OfType<EmailContactSearchObject>();
            var listExistEmails = new List<string>();
            if (resultEmailContact != null && resultEmailContact.Any())
            {
                foreach (var emailContact in resultEmailContact)
                {
                    Result.Add(emailContact);
                    listExistEmails.Add(emailContact.EMail);
                }    
            }
            var resultContacts =
                (_listContactsRules[0] as ISearchRule).GetResults().OperationResult.OfType<ContactSearchObject>();
            if (resultContacts != null && resultContacts.Any())
            {
                foreach (var contactSearchObject in resultContacts)
                {
                    if (!string.IsNullOrEmpty(contactSearchObject.EmailAddress) &&
                        !listExistEmails.Contains(contactSearchObject.EmailAddress))
                    {
                        Result.Add(contactSearchObject);
                    }
                    else if (!string.IsNullOrEmpty(contactSearchObject.EmailAddress2) &&
                             !listExistEmails.Contains(contactSearchObject.EmailAddress2))
                    {
                        Result.Add(contactSearchObject);
                    }
                    else if (!string.IsNullOrEmpty(contactSearchObject.EmailAddress3) &&
                             !listExistEmails.Contains(contactSearchObject.EmailAddress3))
                    {
                        Result.Add(contactSearchObject);
                    }
                }                                
            }
        }

        public override void Reset()
        {
            base.Reset();
            _listContactsRules.ForEach(r => r.Reset());
        }

        public override void Init()
        {
            base.Init();
            _listContactsRules.ForEach(r => r.Init());
        }

    }
}