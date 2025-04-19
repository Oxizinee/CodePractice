using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PapersPlease.Inspector
{
    public class Inspector : MonoBehaviour, IPointerClickHandler
    {
        public bool IsInInspectorMode;
        public GameObject InspectorUI;
        List<InspectableField> selectedField = new List<InspectableField>();

        public delegate void OnInspectorModeChangedDelegate(bool isActive);
        public OnInspectorModeChangedDelegate OnInspectorModeChanged;

        public void OnPointerClick(PointerEventData eventData)
        {
            SwitchInspectorMode();
        }

        private void SwitchInspectorMode()
        {
            IsInInspectorMode = !IsInInspectorMode;

            InspectorUI.SetActive(IsInInspectorMode);
            OnInspectorModeChanged?.Invoke(IsInInspectorMode);
        }

        public void SelectField(InspectableField fieldToAdd)
        {
            selectedField.Add(fieldToAdd);

            if(selectedField.Count == 2 )
            {
                StartCoroutine(RunCorrelationCheck(selectedField[0], selectedField[1]));
            }
        }

        private IEnumerator RunCorrelationCheck(InspectableField a, InspectableField b)
        {
            Debug.Log("Checkig 2 fields...");

            yield return new WaitForSeconds(1);

            if (a.field == b.field && a.value != b.value)
            {
                //trigger ui mismatch
                Debug.Log("Mismatch detected");
            }
            else
            {
                Debug.Log("No correlation detected.");
            }

            selectedField.Clear();

            yield return new WaitForSeconds(1);
            SwitchInspectorMode();
            yield return null;
        }
    }
}