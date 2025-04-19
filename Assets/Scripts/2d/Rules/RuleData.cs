using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PapersPlease.Rules
{
    public enum RuleType
    {
        MustHavePassport,
        EntryPermitRequired,
        BanOnCountry
    }

    [System.Serializable]
    public class RuleData
    {
        public RuleType ruleType;
        public string targetCountry;
    }

    public abstract class Rule
    {
        public RuleType ruleType;
        public abstract bool IsViolated(CitizenProfile citizen, List<Document> documents);
    }

    public class MustHavePassportRule : Rule
    {
        public MustHavePassportRule()
        {
            ruleType = RuleType.MustHavePassport;
        }

        public override bool IsViolated(CitizenProfile citizen, List<Document> documents)
        {
            return documents.OfType<PassportDocument>().FirstOrDefault() == null;
        }


    }

    public class BanOnCountryRule : Rule
    {
        public string BannedCountry;

        public BanOnCountryRule(string bannedCountry)
        {
            BannedCountry = bannedCountry;
            ruleType = RuleType.BanOnCountry;
        }

        public override bool IsViolated(CitizenProfile citizen, List<Document> documents)
        {
            if (citizen.Nationality == BannedCountry)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}