using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
public class CharacterMover : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _moveDuration = 1;
    [Space]
    [Header("Rotate")]
    [SerializeField] private float _rotateSpeed = 60;
    private RaycastHit _raycastMouseHit;
    [Space(10f)]
    [Header("Jump")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance = 4;
    [SerializeField] private float _jumpStrength = 1;
    [SerializeField] private float _jumpHeight = 2;
    [SerializeField] private float _jumpDuration = 1;
    private Rigidbody _rigidBody;
    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        LookAtMousePos();

        if (Input.GetAxis("Vertical") > 0 && IsGrounded())
        {
            _rigidBody.DOMove(transform.position + (transform.forward * (Input.GetAxis("Vertical") * _moveSpeed)), _moveDuration, false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            _rigidBody.DOJump(transform.position + (transform.forward * _jumpStrength), _jumpHeight, 1, _jumpDuration, false)
                .SetEase(Ease.InOutSine);
        }

    }
    private void LookAtMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out _raycastMouseHit))
        {
            GameObject ObjectHit = _raycastMouseHit.transform.gameObject;
            if (ObjectHit != null) 
            {
                Debug.DrawLine(Camera.main.transform.position, _raycastMouseHit.point, Color.blue, 0.5f);
                Vector3 direction = (_raycastMouseHit.point - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z), Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotateSpeed);
            }
        }
    }

    private bool IsGrounded()
    {
        RaycastHit hit; 

        if (Physics.Raycast(transform.position, -Vector3.up * _groundCheckDistance, out hit, 1, _groundLayer))
        {
            Debug.DrawRay(transform.position, -Vector3.up * _groundCheckDistance, Color.red, 1);
            if (hit.collider != null)
            {
                return true;
            }
        }
        return false;
    }

}
