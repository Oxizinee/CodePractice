using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
