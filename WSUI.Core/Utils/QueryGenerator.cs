///////////////////////////////////////////////////////////
//  QueryGenerator.cs
//  Implementation of the Class QueryGenerator
//  Generated by Enterprise Architect
//  Created on:      28-Sep-2013 1:56:28 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using OF.Core.Core.Rules;
using OF.Core.Extensions;
using OF.Core.Interfaces;

namespace OF.Core.Utils
{
    public class QueryGenerator : IQueryGenerator
    {
        private const string QueryTemplate = "{0} {1} {2}";
        private const string SelectTopTemplate = "SELECT TOP {0} ";
        private const string FieldsSeparator = ",";

        protected QueryGenerator()
        {
        }

        private static Lazy<IQueryGenerator> _instance = new Lazy<IQueryGenerator>(() =>
        {
            var inst = new QueryGenerator();
            inst.Init();
            return inst;
        });

        public static IQueryGenerator Instance
        {
            get { return _instance.Value; }
        }

        ///
        /// <param name="type"></param>
        /// <param name="searchCriteria"></param>
        /// <param name="exludeIgnored"></param>
        public string GenerateQuery(Type type, string searchCriteria, int topResult, IRuleQueryGenerator ruleQueryGenerator, bool isAdvancedMode)
        {
            var listFields = FieldCash.Instance.GetFieldList(type);
            if (listFields == null || listFields.Count == 0)
                return "";
            var selectPart = GenerateSelectPart(listFields, topResult);
            var fromPart = GenerateFromPart();
            var wherePart = isAdvancedMode ? GenerateAdvancedWherePart(ruleQueryGenerator,searchCriteria) : GenerateWherePart(ruleQueryGenerator);
            var query = string.Format(QueryTemplate, selectPart, fromPart, wherePart);
            return query;
        }

        private void Init()
        { }

        private string GenerateSelectPart(IList<Tuple<string, int>> list, int top)
        {
            var selectPart = string.Format(SelectTopTemplate, top);
            var fields = string.Join(FieldsSeparator, list.Select(item => item.Item1).ToArray());
            selectPart += fields;
            return selectPart;
        }

        private string GenerateFromPart()
        {
            return "FROM SystemIndex";
        }

        private string GenerateWherePart(IRuleQueryGenerator ruleQueryGenerator)
        {
            if (ruleQueryGenerator == null)
                return "";
            string where = ruleQueryGenerator.GenerateWherePart(RuleFactory.Instance.GetAllRules());
            return where;
        }

        private string GenerateAdvancedWherePart(IRuleQueryGenerator ruleQueryGenerator,string advancedCriteria)
        {
            if (ruleQueryGenerator == null)
                return "";
            string where = ruleQueryGenerator.GenerateAdvancedWherePart();
            return where;
        }

    }//end QueryGenerator
}//end namespace Utils