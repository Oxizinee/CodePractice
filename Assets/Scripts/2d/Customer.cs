using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Events;



public class Customer : MonoBehaviour, IPointerClickHandler
{
    public Transform _talkingPosition;
    public int maxDocumentsToSpawn = 4;
    public float _walkAwayDistance = 10;
    public float _movementSpeed = 10, _positionToReachDistance = 0.05f;
    private bool _isWalkingAway;

    public GameObject DocumentPrefab;
    private GameObject[] _allDocuments;

    public delegate void OnWalkAwayDelegate();

    public OnWalkAwayDelegate OnWalkAway;

    public float ColorChangingDuration = 1, _offsetBetweenDocuments = 1, _documentYOffset = 0.2f;

    private SpriteRenderer _spriteRenderer;
    private Color _mainColor;
    [Header("Customer Info")]
   // [SerializeField] private Nationality _nationality;
    [SerializeField]private int _gender; //0 - male, 1- female

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
        _gender = Random.Range(0, 2);
        if (_gender == 0)
        {
            _mainColor = Color.blue;
        }
        else
        {
            _mainColor = Color.magenta;
        }
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
            GameObject Document = Instantiate(DocumentPrefab, spawnPos, Quaternion.identity, transform);
            Document.GetComponent<Document>().SetDocument(/*_nationality.ToString()*/"nationality", _gender == 0 ? "Male" : "Female");
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

            while (Vector3.Distance(transform.position, targetPos) > _positionToReachDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, _movementSpeed * Time.deltaTime);
                yield return null;

            }

            OnWalkAway?.Invoke();
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
