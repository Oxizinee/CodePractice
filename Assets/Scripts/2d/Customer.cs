using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;

[System.Serializable]
public struct CitizenProfile
{
    public string Name;
    public string Nationality;
    public string Gender;
    public string PassportNumber;
    public DateTime ExpirationDate;
}

public class Customer : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] CitizenProfile profile;
    public Transform _talkingPosition;
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
  

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(WalkAway());
    }

    void Start()
    {
        GenerateCitizenInfo();
        _talkingPosition = GameObject.Find("TalkingPosition").transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(WalkToWindow());
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
        }
        else
        {
            _mainColor = Color.magenta;
            profile.Gender = "F";
            profile.Name = "Eva Kowalska";
            profile.PassportNumber = "E5SHF";
        }
        profile.Nationality = "Czebuka";
        profile.ExpirationDate = DateTime.Today;
    }

    private IEnumerator WalkToWindow()
    {
        _spriteRenderer.color = Color.gray;
        while (Vector3.Distance(transform.position, _talkingPosition.position) > _positionToReachDistance && !_isWalkingAway)
        {
            transform.position = Vector3.MoveTowards(transform.position, _talkingPosition.position, Time.deltaTime * _movementSpeed);
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
            Document.GetComponent<Document>().CitezenProfile = profile;
            _allDocuments[i] = Document.GetComponent<Document>();
        }

        foreach(Document document in _allDocuments)
        {
            document.gameObject.transform.DOMoveY(transform.position.y + (Vector3.down.y * _documentYOffset), 0.5f).SetEase(Ease.OutBack);
        }
    }
    private IEnumerator WalkAway()
    {
        if (CheckForAllDocuments())
        {
            yield return new WaitForSeconds(1);

            _isWalkingAway = true;

            bool canPass = _allDocuments.Any(document => document.GetEntryStatus() == EntryStatus.Aproved);

            Vector3 targetPos = canPass == true ? new Vector3(transform.position.x + _walkAwayDistance, transform.position.y - 4, transform.position.z) 
                : new Vector3(transform.position.x - _walkAwayDistance, transform.position.y - 4, transform.position.z);

            CollectDocuments();

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
    }

    private bool CheckForAllDocuments()
    {
        if (_allDocuments.All(go => !go.gameObject.activeSelf))
        {
            Debug.Log("Thanks for all docs!");
            return true;
        }
        else
        {
            Debug.Log("Can't go, give me my docs!");
            return false;
        }
    }
    private void CollectDocuments()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
