using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

public class DayManager : MonoBehaviour
{
    public DayConfig currentDayConfig;
    public int CurrentDay = 1;
    
    


    void Awake()
    {
        DontDestroyOnLoad(gameObject); //preserve across all scenes
    }

    private void Start()
    {
        LoadDayConfig(CurrentDay);
    }
    void LoadDayConfig(int currentDay)
    {
        string path = Path.Combine(Application.streamingAssetsPath, $"DayConfigs/day_{currentDay}.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentDayConfig = JsonConvert.DeserializeObject<DayConfigWrapper>(json).ToDayConfig();
            Debug.Log($"Day {currentDayConfig.dayNumber} loaded correctly with {currentDayConfig.ruleData.Count} rules.");
            foreach (RuleData rule in currentDayConfig.ruleData)
            {
                Debug.Log(rule.ruleType);
                currentDayConfig.rules.Add(CreateRuleFromData(rule));
            }
        }
        else
        {
            Debug.Log($"Day config for {currentDay} not found");
        }
    }

    public static Rule CreateRuleFromData(RuleData data)
    {
        switch (data.ruleType)
        {
            case RuleType.MustHavePassport:
               return new MustHavePassportRule();

            //case RuleType.BanOnCountry:
            //    return new BanOnCountryRule(data.targetCountry);

                // Add more cases here
        }

        throw new Exception($"Unknown rule type: {data.ruleType}");
    }

}


[System.Serializable]
public class DayConfig
{
    public int dayNumber;
    //list of rules
    public List<RuleData> ruleData;
    public List<Rule> rules = new List<Rule>();
    //list of events
    //list of special customers

    //list of documents that can appear
    public List<DocumentType> documents;
}

public class DayConfigWrapper
{
    public int dayNumber;
    public List<RuleData> rules = new List<RuleData>();
    public List<DocumentType> documents = new List<DocumentType>();
    public DayConfig ToDayConfig()
    {
        return new DayConfig()
        {
            dayNumber = this.dayNumber,
            ruleData = this.rules,
            documents = this.documents
        };
    }
}