    public enum RuleType
    {
        MustHavePassport,
        EntryPermitRequired,
        BanOnCountry
    }

    [System.Serializable]
    public class Rule
    {
        public RuleType ruleType;
        public string targetCountry;
    }
