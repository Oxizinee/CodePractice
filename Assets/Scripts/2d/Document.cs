using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Document : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float DragSpeed = 0.5f, GiveBackOffset = 3;
    public GameObject _officeVersion, _inspectorVersion;
    public TextMeshPro _nationalityText, _genderText;
    int _officeLayerValue, _inspectorLayerValue, _customerLayerValue;

    public bool _isDragging, _canGiveBack;

    private string _nationality, _gender;
    public void OnDrag(PointerEventData eventData)
    {
        _isDragging = true;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.WorldToScreenPoint(transform.position).z));
        transform.position = Vector3.Lerp(transform.position, new Vector3(mousePos.x, mousePos.y, transform.position.z),Time.deltaTime * DragSpeed);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;

        if (_canGiveBack)
        {
            transform.DOMoveY(transform.position.y - GiveBackOffset,0.5f).SetEase(Ease.Linear).OnComplete(() => gameObject.SetActive(false));
            Debug.Log("Gave it back!");
        }
    }

    public void SetDocument(string nationality, string gender)
    {
        _nationality = nationality;
        _gender = gender;
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_isDragging && other.gameObject.layer == _customerLayerValue)
        {
            _canGiveBack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == _customerLayerValue)
        {
            _canGiveBack = false;
        }
    }
    void Start()
    {
        _officeVersion.SetActive(true);
        _inspectorVersion.SetActive(false);

        _officeLayerValue = LayerMask.NameToLayer("Office");
        _inspectorLayerValue = LayerMask.NameToLayer("Inspector");
        _customerLayerValue = LayerMask.NameToLayer("Customer");

        SetTextOnDocument();
    }

    private void SetTextOnDocument()
    {
        _nationalityText.text = _nationality;
        _genderText.text = _gender;
    }
}
