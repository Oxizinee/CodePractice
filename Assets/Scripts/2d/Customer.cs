using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using PapersPlease.Rules;
using System.Collections.Generic;

[System.Serializable]
public struct CitizenProfile
{
    public string Name;
    public string Nationality;
    public string Gender;
    public string PassportNumber;
    public DateTime ExpirationDate;
}

public class Customer : MonoBehaviour
{
    [SerializeField] CitizenProfile profile;
    public Vector3 _talkingPosition;
    public int maxDocumentsToSpawn = 4;
    public float _walkAwayDistance = 10;
    public float _movementSpeed = 10, _positionToReachDistance = 0.05f;
    private bool _isWalkingAway;

    public GameObject[] DocumentPrefabs;
    private Document[] _allDocuments;

    public delegate void OnWalkAwayDelegate();

    public OnWalkAwayDelegate OnWalkAway;

    public float ColorChangingDuration = 1, _offsetBetweenDocuments = 1, _documentYOffset = 0.2f;

    private SpriteRenderer _spriteRenderer;
    private Color _mainColor;
    
    private List<Rule> _currentDayRules = new List<Rule>();

    void Start()
    {
        GenerateCitizenInfo();
        _talkingPosition = GameObject.Find("TalkingPosition").transform.position;
        _talkingPosition = new Vector3(_talkingPosition.x, _talkingPosition.y, -1);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(WalkToWindow());
    }

    public List<Rule> SetCurrentDayRules(List<Rule> currentDayRules)
    {
       return _currentDayRules = currentDayRules;
    }
    private void GenerateCitizenInfo()
    {
        float gender = UnityEngine.Random.value;
        if(gender < 0.5f) 
        {
            _mainColor = Color.blue;
            profile.Gender = "M";
            profile.Name = "John Kowalski";
            profile.PassportNumber = "E5SHM";
            profile.Nationality = "Czebuka";
        }
        else
        {
            _mainColor = Color.magenta;
            profile.Gender = "F";
            profile.Name = "Eva Kowalska";
            profile.PassportNumber = "E5SHF";
            profile.Nationality = "Arasaka";
        }
        profile.ExpirationDate = DateTime.Today;
    }

    private IEnumerator WalkToWindow()
    {
        _spriteRenderer.color = Color.gray;
        while (Vector3.Distance(transform.position, _talkingPosition) > _positionToReachDistance && !_isWalkingAway)
        {
            transform.position = Vector3.MoveTowards(transform.position, _talkingPosition, Time.deltaTime * _movementSpeed);
            yield return null;
        }


        _spriteRenderer.DOColor(_mainColor, ColorChangingDuration);
        yield return new WaitForSeconds(1);

        SpawnDocuments();

        yield return null;
    }

    private void SpawnDocuments()
    {
        int randomAmount = UnityEngine.Random.Range(1, maxDocumentsToSpawn);
        Debug.Log(randomAmount.ToString());
        _allDocuments = new Document[2];
        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnPos = new Vector3(transform.position.x + (i * _offsetBetweenDocuments), transform.position.y, transform.position.z);
            GameObject Document = Instantiate(DocumentPrefabs[i], spawnPos, Quaternion.identity, transform);
            Document.GetComponent<Document>().SetUpCitizenProfile(profile);
            Document.GetComponent<Document>().OnReturnDocument += OnReturnDocument;     
            _allDocuments[i] = Document.GetComponent<Document>();
        }

        foreach(Document document in _allDocuments)
        {
            document.gameObject.transform.DOMoveY(transform.position.y + (Vector3.down.y * _documentYOffset), 0.5f).SetEase(Ease.OutBack);
        }
    }

    private void OnReturnDocument()  // every time you return a doc it checks if it has all documents back if so starts to walk away
    {
        if (HasAllDocumentsBack())
        {
            EvaluateDocuments();
            StartCoroutine(WalkAway());
        }
    }

    private void EvaluateDocuments()
    {
        List<Rule> violations = RuleValidator.ViolatedRules(_currentDayRules,profile,_allDocuments.ToList());

        if (violations.Count > 0)
        {
            foreach (Rule violatedRule in violations)
            {
                Debug.Log($"This citizen violated {violatedRule.ruleType}");
            }
        }
        else
        {
            Debug.Log("No rules were violated");
        }
    }
    private IEnumerator WalkAway()
    {
       
            yield return new WaitForSeconds(1);

            _isWalkingAway = true;

            bool canPass = _allDocuments.Any(document => document.GetEntryStatus() == EntryStatus.Aproved);

            Vector3 targetPos = canPass == true ? new Vector3(transform.position.x + _walkAwayDistance, transform.position.y - 4, transform.position.z) 
                : new Vector3(transform.position.x - _walkAwayDistance, transform.position.y - 4, transform.position.z);

            _spriteRenderer.DOColor(Color.gray, ColorChangingDuration);
            yield return new WaitForSeconds(1);

            OnWalkAway?.Invoke();

            while (Vector3.Distance(transform.position, targetPos) > _positionToReachDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, _movementSpeed * Time.deltaTime);
                yield return null;

            }
           
            Destroy(gameObject);
    }

    private bool HasAllDocumentsBack()
    {
        if (_allDocuments.All(go => go.IsReturned()))
        {
            Debug.Log("Gonna go now!");
            return true;
        }
        else
        {
            return false;
        }
    }
}
