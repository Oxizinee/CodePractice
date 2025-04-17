using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StamperButton : MonoBehaviour, IPointerClickHandler
{
    public GameObject Stampers;
    public float newPositionOffset = 30;
    public bool _stampersOut = false;
    private float _originalPosition;
    private void Awake()
    {
        _originalPosition = Stampers.transform.position.x;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!_stampersOut)
        {
            Stampers.transform.DOMoveX(_originalPosition - newPositionOffset, 1).SetEase(Ease.OutExpo).OnComplete(() => _stampersOut = true);
        }
        else
        {
            Stampers.transform.DOMoveX(_originalPosition, 1).SetEase(Ease.InOutBack).OnComplete(() => _stampersOut = false);
        }

    }
}
