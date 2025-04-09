using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodControl : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask _panLayer;
    public bool isTouchingPan;
    private Vector3 _panPosition, _lastPanPosition;
    RaycastHit hit;
    void Start()
    {
        _lastPanPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
       if (Physics.Raycast(transform.position, Vector3.down * 0.1f, out hit, 0.1f, _panLayer, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(transform.position, Vector3.down * 0.1f, Color.red, 0.1f);

            if(hit.collider != null)
            {
                _panPosition = hit.point;
                isTouchingPan = true;
                transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            }

            // float heightOffset = transform.localScale.y / 2f;
            // transform.position = new Vector3(_panPosition.x, _panPosition.y + heightOffset, _panPosition.z);

        }
        else
        {
            isTouchingPan = false;
          
        }
    }

    private void LateUpdate()
    {
       if(isTouchingPan) 
        {
        }
    }
}
