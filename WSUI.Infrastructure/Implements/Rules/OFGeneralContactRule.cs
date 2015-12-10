using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OF.Core.Core.Search;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;
using System.Text.RegularExpressions;
using Microsoft.Practices.Unity;
using OF.Core;
using OF.Core.Data.ElasticSearch;

namespace OF.Infrastructure.Implements.Rules
{
    public class OFGeneralContactRule : BaseSearchRule<OFBaseSearchObject,OFEmail>
    {
        #region [needs]

        private int _first, _second;
        private List<string> _listExistingEmails; 

        private readonly IList<ISearch> _listContactsRules = new List<ISearch>();

        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";
        #endregion

        public OFGeneralContactRule(int firstTime, int secondTime, IUnityContainer container)
            :this(firstTime,secondTime,null,container)
        {
        }

        public OFGeneralContactRule(int firstTime, int secondTime, object lockExternal, IUnityContainer container)
            :base(lockExternal,false,container)
        {
            ConstructorInit(container);
            _first = firstTime;
            _second = secondTime;
        }

        public OFGeneralContactRule(object lockObject, IUnityContainer container)
            :base(lockObject,false,container)
        {
            ConstructorInit(container);
        }

        private void ConstructorInit(IUnityContainer container)
        {
            Priority = 0;
            _listContactsRules.Add(new OFContactSearchRule(Lock,container));
            _listContactsRules.Add(new OFEmailContactSearchRule(Lock,container));
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
                OFLogger.Instance.LogError("GeneralContact: {0}", ex.ToString());
            }
            finally
            {
                Event.Set();
            }
        }

        protected override void ProcessResult()
        {
            var arrQuery = InitQueryWords(Query);
            var resultContacts = (_listContactsRules[0] as ISearchRule).GetResults().OperationResult.OfType<OFContactSearchObject>();
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

            var resultEmailContact = (_listContactsRules[1] as ISearchRule).GetResults().OperationResult.OfType<OFEmailContactSearchObject>();
            if (resultEmailContact != null && resultEmailContact.Any())
            {
                foreach (var emailContact in resultEmailContact)
                {
                    if ( _listExistingEmails.Contains(emailContact.EMail.ToLowerInvariant())) 
                        continue;
                    Result.Add(emailContact);
                    _listExistingEmails.Add(emailContact.EMail.ToLowerInvariant());
                }
            }
            SetTypeResult(_listContactsRules.OfType<ISearchRule>().Any(r => r.GetResults().Type == OFTypeResult.Error) ? OFTypeResult.Error : OFTypeResult.Ok);
            _listContactsRules.OfType<ISearchRule>().SelectMany(r => r.GetResults().Messages.Select(m => m.Message)).ForEach(AddMessage);

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