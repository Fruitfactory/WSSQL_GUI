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
using WSUI.Core.Data.ElasticSearch;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class GeneralContactRule : BaseSearchRule<ContactSearchObject,WSUIStub>
    {
        #region [needs]

        private int _first, _second;
        private List<string> _listExistingEmails; 

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
            //_listContactsRules.Add(new EmailContactSearchRule(Lock)); // TODO implement sorting for email via contacts
            _listExistingEmails = new List<string>();
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
            var arrQuery = InitQueryWords(Query);
            var resultContacts = (_listContactsRules[0] as ISearchRule).GetResults().OperationResult.OfType<ContactSearchObject>();
            if (resultContacts != null && resultContacts.Any())
            {
                foreach (var contactSearchObject in resultContacts)
                {
                    var contactName = string.Format("{0} {1}", contactSearchObject.FirstName, contactSearchObject.LastName);

                    if (IsEmail(contactSearchObject.EmailAddress1) && 
                        ( IsContainsSearchCriterias(contactName, arrQuery) || IsContainsSearchCriterias(contactSearchObject.EmailAddress1, arrQuery)) &&
                        !_listExistingEmails.Contains(contactSearchObject.EmailAddress1.ToLowerInvariant()))
                    {
                        Result.Add(contactSearchObject);
                        _listExistingEmails.Add(contactSearchObject.EmailAddress1.ToLowerInvariant());
                    }
                    else if (IsEmail(contactSearchObject.EmailAddress2) &&
                        (IsContainsSearchCriterias(contactName, arrQuery) || IsContainsSearchCriterias(contactSearchObject.EmailAddress2, arrQuery)) &&
                             !_listExistingEmails.Contains(contactSearchObject.EmailAddress2.ToLowerInvariant()))
                    {
                        contactSearchObject.EmailAddress1 = contactSearchObject.EmailAddress2;
                        Result.Add(contactSearchObject);
                        _listExistingEmails.Add(contactSearchObject.EmailAddress2.ToLowerInvariant());
                    }
                    else if (IsEmail(contactSearchObject.EmailAddress3) &&
                        (IsContainsSearchCriterias(contactName, arrQuery) || IsContainsSearchCriterias(contactSearchObject.EmailAddress3, arrQuery)) &&
                             !_listExistingEmails.Contains(contactSearchObject.EmailAddress3.ToLowerInvariant()))
                    {
                        contactSearchObject.EmailAddress1 = contactSearchObject.EmailAddress3;
                        Result.Add(contactSearchObject);
                        _listExistingEmails.Add(contactSearchObject.EmailAddress3.ToLowerInvariant());
                    }
                    else if (!string.IsNullOrEmpty(contactSearchObject.FirstName) && !string.IsNullOrEmpty(contactSearchObject.LastName) &&
                        !_listExistingEmails.Contains(contactName) && string.IsNullOrEmpty(contactSearchObject.EmailAddress1) && string.IsNullOrEmpty(contactSearchObject.EmailAddress2) && string.IsNullOrEmpty(contactSearchObject.EmailAddress3))
                    {
                        Result.Add(contactSearchObject);
                        _listExistingEmails.Add(contactName);
                    }
                }
            }

            //TODO: add seraching by email addresses.
            //var resultEmailContact = (_listContactsRules[1] as ISearchRule).GetResults().OperationResult.OfType<EmailContactSearchObject>();
            //if (resultEmailContact != null && resultEmailContact.Any())
            //{
            //    foreach (var emailContact in resultEmailContact)
            //    {
            //        if(!IsEmail(emailContact.EMail) || _listExistingEmails.Contains(emailContact.EMail.ToLowerInvariant()))
            //            continue;
            //        Result.Add(emailContact);
            //        _listExistingEmails.Add(emailContact.EMail.ToLowerInvariant());
            //    }    
            //}
        }

        private bool IsContainsSearchCriterias(string emailAddress, string[] arrQuery)
        {
            return arrQuery != null && arrQuery.Length > 0 && arrQuery.Any(s => emailAddress.IndexOf(s,StringComparison.InvariantCultureIgnoreCase) > -1);
        }

        private string[] InitQueryWords(string query)
        {
            return Query.Split(' ');
        }

        public override void Reset()
        {
            base.Reset();
            _listContactsRules.ForEach(r => r.Reset());
            ClearLisOfExistingEmails();
        }

        public override void Init()
        {
            ObjectType = RuleObjectType.Contact;
            base.Init();
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

        private void ClearLisOfExistingEmails()
        {
            if (_listExistingEmails == null)
                return;
            _listExistingEmails.Clear();
        }

    }
}