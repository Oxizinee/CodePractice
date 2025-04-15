using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum DocumentType
{
    Passport,
    IDCard,

}

public abstract class Document : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //in game representatives
    public DocumentType DocumentType;
    public GameObject _officeVersion, _inspectorVersion;
   
    public CitezenProfile CitezenProfile;
    
    public Dictionary<string, string> Fields = new Dictionary<string, string>();
    

    private float _dragSpeed, _giveBackOffset;
    
    //dragging
    private bool _isDragging, _canGiveBack;

    //collision layers
    private int _officeLayerValue, _inspectorLayerValue, _customerLayerValue;

    public void Start()
    {
        _dragSpeed = 40;
        _giveBackOffset = 3;

        _officeVersion.SetActive(true);
        _inspectorVersion.SetActive(false);

        _officeLayerValue = LayerMask.NameToLayer("Office");
        _inspectorLayerValue = LayerMask.NameToLayer("Inspector");
        _customerLayerValue = LayerMask.NameToLayer("Customer");

        SetTextOnDocument();

    }

    public virtual void SetTextOnDocument() { }
    public Document(DocumentType documentType, CitezenProfile citezenProfile) 
    { 
        DocumentType = documentType;
        CitezenProfile = citezenProfile;
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

        if (_canGiveBack)
        {
            transform.DOMoveY(transform.position.y - _giveBackOffset, 0.5f).SetEase(Ease.Linear).OnComplete(() => gameObject.SetActive(false));
            Debug.Log("Gave it back!");
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (_isDragging && other.gameObject.layer == _customerLayerValue)
        {
            _canGiveBack = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == _inspectorLayerValue)
        {
            Debug.Log("In Inspector window");
            _inspectorVersion.SetActive(true);
            _officeVersion.SetActive(false);
        }
        else if (other.gameObject.layer == _officeLayerValue)
        {
            Debug.Log("In office window");
            _inspectorVersion.SetActive(false);
            _officeVersion.SetActive(true);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == _customerLayerValue)
        {
            _canGiveBack = false;
        }
    }
}
public class PassportDocument : Document
{
    public TextMeshPro _nameText, _nationalityText, _genderText;
    public PassportDocument(DocumentType documentType, CitezenProfile citezenProfile) : base(documentType, citezenProfile)
    {
        Fields["Name"] = citezenProfile.Name;
        Fields["Nationality"] = citezenProfile.Nationality;
        Fields["Gender"] = citezenProfile.Gender;
    }

    public override void SetTextOnDocument()
    {
        _nameText.text = CitezenProfile.Name;
        _nationalityText.text = CitezenProfile.Nationality;
        _genderText.text = CitezenProfile.Gender;
    }
    

}
