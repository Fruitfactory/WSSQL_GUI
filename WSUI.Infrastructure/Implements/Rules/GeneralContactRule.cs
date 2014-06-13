using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using System.Text.RegularExpressions;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class GeneralContactRule : BaseSearchRule<BaseSearchObject>
    {
        #region [needs]

        private int _first, _second;

        private readonly IList<ISearch> _listContactsRules = new List<ISearch>();

        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";
        #endregion

        public GeneralContactRule(int firstTime, int secondTime)
            :this(firstTime,secondTime,null)
        {
        }

        public GeneralContactRule(int firstTime, int secondTime,object lockExternal)
            :base(lockExternal,false)
        {
            ConstructorInit();
            _first = firstTime;
            _second = secondTime;
        }

        public GeneralContactRule(object lockObject)
            :base(lockObject,false)
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
                    if(listExistEmails.Contains(emailContact.EMail.ToLowerInvariant()))
                        continue;
                    Result.Add(emailContact);
                    listExistEmails.Add(emailContact.EMail.ToLowerInvariant());
                }    
            }
            var resultContacts =
                (_listContactsRules[0] as ISearchRule).GetResults().OperationResult.OfType<ContactSearchObject>();
            if (resultContacts != null && resultContacts.Any())
            {
                foreach (var contactSearchObject in resultContacts)
                {
                    if (IsEmail(contactSearchObject.EmailAddress) &&
                        !listExistEmails.Contains(contactSearchObject.EmailAddress.ToLowerInvariant()))
                    {
                        Result.Add(contactSearchObject);
                        listExistEmails.Add(contactSearchObject.EmailAddress.ToLowerInvariant());
                    }
                    else if (IsEmail(contactSearchObject.EmailAddress2) &&
                             !listExistEmails.Contains(contactSearchObject.EmailAddress2.ToLowerInvariant()))
                    {
                        Result.Add(contactSearchObject);
                        listExistEmails.Add(contactSearchObject.EmailAddress2.ToLowerInvariant());
                    }
                    else if (IsEmail(contactSearchObject.EmailAddress3) &&
                             !listExistEmails.Contains(contactSearchObject.EmailAddress3.ToLowerInvariant()))
                    {
                        Result.Add(contactSearchObject);
                        listExistEmails.Add(contactSearchObject.EmailAddress3.ToLowerInvariant());
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
            ObjectType = RuleObjectType.Contact;
            _listContactsRules.ForEach(r =>
            {
                r.Init();
                r.SetProcessingRecordCount(_first,_second);
            });
        }

        private bool IsEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase) ;
        }

    }
}