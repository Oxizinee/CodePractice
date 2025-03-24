using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using Unity.Burst.CompilerServices;
using UnityEngine.SocialPlatforms.Impl;
public class CharacterMover : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _accelerationRate = 5;
    [SerializeField] private float _deaccelerationRate = 5;
    private Vector3 _targetVelocity;
    [SerializeField] private float DebugVel;

    [Space]
    [Header("Rotate")]
    [SerializeField] private float _rotateSpeed = 60;
    [SerializeField] private GameObject _armatureToTilt;
    [Range(0,0.1f)]
    [SerializeField] private float _tiltRotationSpeed = 1f;
    [SerializeField] private float _tiltRotationBackUp = 1f;
    [SerializeField] private Vector2 _minMaxTiltAngle = new Vector2(-90, 60);
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
        TiltRotation();

        DebugVel = _rigidBody.velocity.magnitude;


        if (Input.GetAxis("Vertical") > 0 && IsGrounded())
        {
            _targetVelocity = transform.forward * _moveSpeed;
        }

        if (IsGrounded()) // apply drag on the ground
        {
            _targetVelocity *= (1 - Time.deltaTime * _rigidBody.drag);
        }
     
    
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            _shouldJump = true;
        }
    }

    private void TiltRotation()
    {
        Vector3 characterForward = _armatureToTilt.transform.forward;
        Vector3 dragPoint = new Vector3(transform.position.x - (transform.forward.x * 1.5f), 1, transform.position.z - (transform.forward.z * 1.5f));
        Vector3 dragObjectDirection = (dragPoint - transform.position).normalized;

        float dot = Vector3.Dot(characterForward, dragObjectDirection);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        Quaternion objectRotation = Quaternion.Euler(angle * -1, 0, 0);
        Quaternion targetRotation = _rigidBody.velocity.magnitude > _moveSpeed / 2 && Input.GetAxis("Vertical") > 0 ? AlignToGround() * objectRotation : AlignToGround();

        _armatureToTilt.transform.localRotation = Quaternion.Slerp(_armatureToTilt.transform.localRotation, targetRotation, 
            (Input.GetAxis("Vertical") > 0 ? _tiltRotationSpeed : _tiltRotationBackUp) * Time.deltaTime) ;
    }
    private void FixedUpdate()
    {
        if (_shouldJump)
        {
            PerformJump();
            _shouldJump = false;
        }

        HandleGroundMovement();
    }

    private void HandleGroundMovement()
    {
        Vector3 newVelocity = Vector3.MoveTowards(new Vector3(_rigidBody.velocity.x, 0, _rigidBody.velocity.z), _targetVelocity,
                                             (Input.GetAxis("Vertical") > 0 ? _accelerationRate : _deaccelerationRate) * Time.deltaTime);

        _rigidBody.velocity = new Vector3(newVelocity.x, _rigidBody.velocity.y, newVelocity.z);
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

    private Quaternion AlignToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, _groundCheckDistance, _groundLayer))
        {
            return Quaternion.FromToRotation(Vector3.up, hit.normal);
        }

        return Quaternion.identity;
    }

}
