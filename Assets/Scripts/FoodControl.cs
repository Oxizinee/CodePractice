using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodControl : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask _panLayer;
    public bool isTouchingPan;
    RaycastHit hit;

    [Header("Cooking")]
    public float CookingValue = 0;
    public float ScaleWhenReady = 2, ScaleDuration = 1;
    Vector3 _originalSize;
    Tween scaleTween;
    bool _isCompleted;
    void Start()
    {
        CookingValue = 0;
        _originalSize = transform.localScale;
        _isCompleted = false;
    }

    // Update is called once per frame
    void Update()
    {
       if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - transform.localScale.y/2, transform.position.z), 
           Vector3.down * 0.05f, out hit, 0.05f, _panLayer, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - transform.localScale.y / 2, transform.position.z)
                , Vector3.down * 0.05f, Color.red, 0.05f);

            if(hit.collider != null)
            {
                isTouchingPan = true;
                if (transform.parent == null)
                {
                    transform.SetParent(hit.transform);
                }
                CookingValue = Mathf.Clamp01(CookingValue + 0.1f * Time.deltaTime );
            }

        }
        else
        {
            isTouchingPan = false;
        }


       if (CookingValue >= 1 && !_isCompleted)
        {
            scaleTween = transform.DOScale(_originalSize * ScaleWhenReady, ScaleDuration)
                .OnComplete(() => transform.DOScale(_originalSize, ScaleWhenReady).OnComplete(() => scaleTween.Kill()));
            Debug.Log("Food is cooked!");
            _isCompleted = true;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if(transform.parent != null)
        {
            transform.SetParent(null);
        }
    }
}
