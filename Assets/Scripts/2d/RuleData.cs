using System.Collections.Generic;
using System.Linq;

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
        public abstract string GetViolationMessage();
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

        public override string GetViolationMessage()
        {
            return "Entry denied: Passport is missing.";
        }
    }