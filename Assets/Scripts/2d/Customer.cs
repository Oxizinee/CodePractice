using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

[System.Serializable]
public struct CitezenProfile
{
    public string Name;
    public string Nationality;
    public string Gender;
    public string IDNumber;
}

public class Customer : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] CitezenProfile profile;
    public Transform _talkingPosition;
    public int maxDocumentsToSpawn = 4;
    public float _walkAwayDistance = 10;
    public float _movementSpeed = 10, _positionToReachDistance = 0.05f;
    private bool _isWalkingAway;

    public GameObject[] DocumentPrefabs;
    private GameObject[] _allDocuments;

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
        SetCustomerInfo();
        _talkingPosition = GameObject.Find("TalkingPosition").transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(WalkToWindow());
    }

    private void SetCustomerInfo()
    {
        float gender = Random.value;
        _mainColor = gender < 0.5 ? Color.blue : Color.magenta;
        profile.Gender = gender < 0.5 ? "M" : "F";
        profile.Nationality = "Czebuka";
        profile.Name = "John Kowalski";
        //_nationality = (Nationality)System.Enum.GetValues(typeof(Nationality)).GetValue(Random.Range(0, System.Enum.GetValues(typeof(Nationality)).Length));

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
        int randomAmount = Random.Range(1, maxDocumentsToSpawn);
        _allDocuments = new GameObject[randomAmount];
        for (int i = 0; i < randomAmount; i++)
        {
            Vector3 spawnPos = new Vector3(transform.position.x + (i * _offsetBetweenDocuments), transform.position.y, transform.position.z);
            GameObject Document = Instantiate(DocumentPrefabs[0], spawnPos, Quaternion.identity, transform);
            Document.GetComponent<PassportDocument>().CitezenProfile = profile;
            _allDocuments[i] = Document;
        }

        foreach(GameObject document in _allDocuments)
        {
            document.transform.DOMoveY(transform.position.y + (Vector3.down.y * _documentYOffset), 0.5f).SetEase(Ease.OutBack);
        }
    }
    private IEnumerator WalkAway()
    {
        if (CheckForAllDocuments())
        {
            yield return new WaitForSeconds(1);

            _isWalkingAway = true;
            Vector3 targetPos = new Vector3(transform.position.x + _walkAwayDistance, transform.position.y - 4, transform.position.z);

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
        if (_allDocuments.All(go => !go.activeSelf))
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
