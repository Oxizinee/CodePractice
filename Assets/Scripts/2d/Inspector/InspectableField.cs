using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PapersPlease.Inspector
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class InspectableField : MonoBehaviour, IPointerClickHandler
    {
        public string field;
        public string value;
        public CitizenProfile profile;
        [SerializeField]private Document _documentReference;
        private Inspector _inspector;
        private void Start()
        {
            _inspector = FindObjectOfType<Inspector>();
            _inspector.OnInspectorModeChanged += ChangeDocumentColliders;
        }

        public void ChangeDocumentColliders(bool isActive)
        {
            _documentReference.GetComponentInChildren<BoxCollider2D>().enabled = !isActive;
           // Debug.Log("STUFF CHANGED" + _documentReference.GetComponentInChildren<BoxCollider2D>().enabled);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if(_inspector.IsInInspectorMode) 
            {
                _inspector.SelectField(this);
                Debug.Log($"{gameObject.name} selected");

            }
        }
        //private void OnDisable()
        //{
        //   // _inspector.OnInspectorModeChanged -= ChangeDocumentColliders;
        //}
        private void OnEnable()
        {
            _documentReference = GetComponentInParent<Document>();
            profile = _documentReference.CitezenProfile;

            value = GetComponent<TextMeshPro>().text;
            field = _documentReference.Fields.FirstOrDefault(x => x.Value.Trim() == value.Trim()).Key;
        }

    }
}