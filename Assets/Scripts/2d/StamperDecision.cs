using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StamperDecision : MonoBehaviour, IPointerClickHandler
{
    public enum StamperType
    {
        Approve,
        Deny
    }

    public StamperType Type;
    public Sprite AcceptSprite, DenySprite;
    int stampLayer;
    public float RayDistance = 2, positionOffset = 5, stampDuration =0.2f;
    private float _originalPosition;
    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOLocalMoveY(_originalPosition - positionOffset, stampDuration).SetEase(Ease.OutBack)
            .OnComplete(() => transform.DOLocalMoveY(_originalPosition, stampDuration).SetEase(Ease.OutBack));

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down * RayDistance, RayDistance);
        foreach (RaycastHit2D h in hits)
        {
            if (h.collider.gameObject.layer == stampLayer)
            {
                if (Type == StamperType.Approve)
                {
                    h.collider.GetComponentInParent<Document>().SetEntryStatus(EntryStatus.Aproved, AcceptSprite, Color.green);
                }
                else
                {
                    h.collider.GetComponentInParent<Document>().SetEntryStatus(EntryStatus.Denied, DenySprite, Color.red);
                }

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        stampLayer = LayerMask.NameToLayer("Stamp");
        _originalPosition = transform.localPosition.y;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, Vector2.down * RayDistance);
    }

}
