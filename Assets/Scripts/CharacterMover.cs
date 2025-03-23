using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
public class CharacterMover : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float _moveSpeed = 5;
    [Space]
    [Header("Rotate")]
    [SerializeField] private float _rotateSpeed = 60;
    private RaycastHit _raycastMouseHit;
    [Space(10f)]
    [Header("Jump")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance = 4;
    [SerializeField] private float _forwardJumpMultiplier = 1.5f;
    [SerializeField] private float _jumpHeightBoost = 1f;
    [SerializeField] private Vector2 _minMaxJumpHeight = new Vector2(1,3);

    private float _acceleration; 
    private Vector3 _previousVelocity;
    private bool _shouldJump;

    private Rigidbody _rigidBody;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _acceleration = (_rigidBody.velocity.magnitude - _previousVelocity.magnitude) / Time.deltaTime;
        _previousVelocity = _rigidBody.velocity;

        LookAtMousePos();

        if (Input.GetAxis("Vertical") > 0 && IsGrounded())
        {
            _rigidBody.velocity = new Vector3(transform.forward.x * _moveSpeed, _rigidBody.velocity.y, transform.forward.z * _moveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            _shouldJump = true;
        }
    }

    private void FixedUpdate()
    {
        if (_shouldJump)
        {
            PerformJump();
            _shouldJump=false;
        }
    }

    private void PerformJump()
    {
        float jumpHeight = Mathf.Clamp(Mathf.Abs(_acceleration) * _jumpHeightBoost, _minMaxJumpHeight.x, _minMaxJumpHeight.y);
        Vector3 forwardBoost = transform.forward * _rigidBody.velocity.magnitude * _forwardJumpMultiplier;

        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, jumpHeight, _rigidBody.velocity.z);
        _rigidBody.velocity += forwardBoost;
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
