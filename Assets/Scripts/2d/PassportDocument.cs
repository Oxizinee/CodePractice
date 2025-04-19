using DG.Tweening;
using PapersPlease.Inspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
public enum EntryStatus
{
    Unset,
    Denied,
    Aproved
}
public enum DocumentType
{
    Passport,
    EntryPermit
}

public abstract class Document : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //in game representatives
    public DocumentType DocumentType;
    [SerializeField] private GameObject _officeVersion, _inspectorVersion, _entryStamp;
   
    public CitizenProfile CitezenProfile;
    
    public Dictionary<string, string> Fields = new Dictionary<string, string>();
    
    private EntryStatus _entryStatus;

    private float _dragSpeed, _giveBackOffset;
    //dragging
   private bool _isDragging, _hoveringAboveCitizen, _isWithCitizen;

    //collision layers
    private int _officeLayerValue, _inspectorLayerValue, _customerLayerValue;

    //Return doc
    public delegate void OnReturnDocumentDelegate();

    public OnReturnDocumentDelegate OnReturnDocument;

    [SerializeField]private InspectableField[] _inspectableFields;

    public void Start()
    {
       _inspectableFields = _inspectorVersion.GetComponentsInChildren<InspectableField>();

        if (_entryStamp != null)
        {
            _entryStamp.SetActive(false);
        }
        else
        {
            _entryStatus = EntryStatus.Denied;
        }
        _dragSpeed = 40;
        _giveBackOffset = 3;
        _isWithCitizen = false;
        _officeVersion.SetActive(true);
        _inspectorVersion.SetActive(false);

        _officeLayerValue = LayerMask.NameToLayer("Office");
        _inspectorLayerValue = LayerMask.NameToLayer("Inspector");
        _customerLayerValue = LayerMask.NameToLayer("Customer");

        SetTextOnDocument();
    }


    public virtual void SetTextOnDocument() {}
    public virtual void SetUpCitizenProfile(CitizenProfile citezenProfile)
    {
        CitezenProfile = citezenProfile;
    }
    public bool IsReturned()
    {
        return _isWithCitizen;
    }
    public void OnDrag(PointerEventData eventData)
    {
        _isDragging = true;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.WorldToScreenPoint(transform.position).z));
        transform.position = Vector3.Lerp(transform.position, new Vector3(mousePos.x, mousePos.y, transform.position.z), Time.deltaTime * _dragSpeed);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;

        if (_hoveringAboveCitizen && _entryStatus != EntryStatus.Unset)
        {
            transform.DOMoveY(transform.position.y - _giveBackOffset, 0.5f).SetEase(Ease.Linear).OnComplete(() => gameObject.SetActive(false));
            _isWithCitizen = true;
            Debug.Log($"Thanks for giving {gameObject.name} back");
            OnReturnDocument?.Invoke();
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (_isDragging && other.gameObject.layer == _customerLayerValue)
        {
            _hoveringAboveCitizen = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == _inspectorLayerValue)
        {
            //Debug.Log("In Inspector window");
            _inspectorVersion.SetActive(true);
            _officeVersion.SetActive(false);
           
        }
        else if (other.gameObject.layer == _officeLayerValue)
        {
           // Debug.Log("In office window");
            _inspectorVersion.SetActive(false);
            _officeVersion.SetActive(true);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == _customerLayerValue)
        {
            _hoveringAboveCitizen = false;
        }
    }

    public void SetEntryStatus(EntryStatus entryStatus, Sprite stampSprite, Color stampColor)
    {
        _entryStatus = entryStatus;
        if (_entryStamp != null)
        {
            _entryStamp.SetActive(true);
            _entryStamp.GetComponent<SpriteRenderer>().sprite = stampSprite;
            _entryStamp.GetComponent<SpriteRenderer>().color = stampColor;
        }
        Debug.Log(gameObject.name + entryStatus.ToString());
    }

    public EntryStatus GetEntryStatus()
    {
        return _entryStatus;
    }
}
public class PassportDocument : Document
{
    [SerializeField] private TextMeshPro _nameText, _nationalityText, _genderText, _passportNumberText, _expirationDateText;
    public override void SetUpCitizenProfile(CitizenProfile citezenProfile) 
    {
        base.SetUpCitizenProfile(citezenProfile);
        Fields["Name"] = citezenProfile.Name;
        Fields["Nationality"] = citezenProfile.Nationality;
        Fields["Gender"] = citezenProfile.Gender;
        Fields["PassportNumber"] = citezenProfile.PassportNumber;
        Fields["ExpirationDate"] = citezenProfile.ExpirationDate.ToShortDateString();

    }

    public override void SetTextOnDocument()
    {
        _nameText.text = CitezenProfile.Name;
        _nationalityText.text = CitezenProfile.Nationality;
        _genderText.text = CitezenProfile.Gender;
        _passportNumberText.text = CitezenProfile.PassportNumber;
        _expirationDateText.text = CitezenProfile.ExpirationDate.ToShortDateString();
    }
    

}
