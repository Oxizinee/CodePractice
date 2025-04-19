using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapersPlease.Rules
{
    public class RuleValidator
    {
        public static List<Rule> ViolatedRules(List<Rule> currentRules, CitizenProfile citizenProfile, List<Document> allDocuments)
        {
            List<Rule> brokenRules = new List<Rule>();

            foreach (var rule in currentRules)
            {
                if (rule.IsViolated(citizenProfile, allDocuments))
                {
                    brokenRules.Add(rule);
                }

            }

            return brokenRules;
        }
    }
}