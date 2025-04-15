using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

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
            Debug.Log($"Day {currentDayConfig.dayNumber} loaded correctly with {currentDayConfig.rules.Count} rules.");
            foreach (Rule rule in currentDayConfig.rules)
            {
                Debug.Log(rule.ruleType);
            }
        }
        else
        {
            Debug.Log($"Day config for {currentDay} not found");
        }
    }
    
   
}
[System.Serializable]
public class DayConfig
{
    public int dayNumber;
    //list of rules
    public List<Rule> rules;
    //list of events
    //list of special customers

    //list of documents that can appear
    public List<DocumentType> documents;
}

public class DayConfigWrapper
{
    public int dayNumber;
    public List<Rule> rules = new List<Rule>();
    public List<DocumentType> documents = new List<DocumentType>();
    public DayConfig ToDayConfig()
    {
        return new DayConfig()
        {
            dayNumber = this.dayNumber,
            rules = this.rules,
            documents = this.documents
        };
    }
}