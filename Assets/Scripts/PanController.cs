using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PanController : MonoBehaviour
{
    Camera _mainCam;
    [Header("Moving Pan")]
    [Space]
    public float _moveSpeed = 2, _minDistanceToMove = 5, minSpeed = 0, maxSpeed = 5;
    public float ClampPosZ = 0.5f;
    Vector3 _originalPos;
    [Header("Rotate Pan")]
    [Space]
    public bool _allowCameraTilt;
    public float _rotationSpeed = 30, _maxTiltAngle = 30;
    public Transform _panPivot;
    float _originalDistance, rotationSpeed;
    public GameObject _foodPrefab;
    [Header("Swing Pan")]
    [Space]
    [SerializeField] private float swingAngle = 30f;
    [SerializeField] private float swingSpeed = 200f;
    [SerializeField] private float swingResetSpeed = 100f;

    private bool _isSwingingUp = false;
    [SerializeField]private float _currentSwingAngle = 0f;

    [Header("FoodDetection")]
    public List<Rigidbody> FoodOnPan = new List<Rigidbody>();

    [Header("FoodOnPan")]
    public float ForceToAdd = 1;
    public float JumpTime = 1;
    public float JumpEndPosAbove = 0.3f;
    public Ease JumpEase = Ease.Linear;

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null && !FoodOnPan.Contains(rb))
        {
            FoodOnPan.Add(rb);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null && FoodOnPan.Contains(rb))
        {
            FoodOnPan.Remove(rb);
        }
    }
    void Start()
    {
        _mainCam = Camera.main;
        _originalDistance = Vector3.Distance(_mainCam.transform.position, transform.position);
        _originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldMousePos = FollowMousePos();

        PanRotation(worldMousePos);

        if (Input.GetMouseButtonDown(1))
        {
            Instantiate(_foodPrefab, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        }

        if (Input.GetMouseButtonDown(0))
        {
           // AddForceToRb();
           foreach (Rigidbody rb in FoodOnPan)
            {
                DG.Tweening.Sequence jumpAndFlip = DOTween.Sequence();
                //rb.transform.DOJump(rb.transform.position + (Vector3.up * JumpEndPosAbove), ForceToAdd, 1, JumpTime, false)
                //     .SetEase(JumpEase);
                jumpAndFlip.Append(
                    rb.transform.DOJump(
                        rb.transform.position + (Vector3.up * JumpEndPosAbove),
                        ForceToAdd,
                        1,
                        JumpTime,
                        false
                    ).SetEase(JumpEase)
                );

                jumpAndFlip.Join(
                    rb.transform.DORotate(
                        rb.transform.eulerAngles + new Vector3(180f, 0, 0f), 
                        JumpTime,
                        RotateMode.FastBeyond360
                    ).SetEase(JumpEase)
                );
            }

            _isSwingingUp = true;
            Debug.Log("Force Added:" + transform.forward * ForceToAdd);
        }

        SwingPan();

    }


    private void AddForceToRb()
    {
        foreach (Rigidbody rb in FoodOnPan)
        {
            rb.AddForceAtPosition(transform.forward * ForceToAdd, rb.transform.localPosition, ForceMode.Impulse);
            rb.transform.SetParent(null);
        }
    }

    private void SwingPan()
    {
        if (_isSwingingUp)
        {
             _currentSwingAngle = Mathf.MoveTowards(_currentSwingAngle, -swingAngle, Time.deltaTime * swingSpeed);

            if (Mathf.Approximately(_currentSwingAngle, -swingAngle))
            {
                _isSwingingUp = false;
            }
        }
        else
        {
             _currentSwingAngle = Mathf.MoveTowards(_currentSwingAngle, 0f, Time.deltaTime * swingResetSpeed);
        }
    }
    private void PanRotation(Vector3 worldMousePos)
    {
        Vector3 worldDirToMouse = worldMousePos - _panPivot.transform.position;
        // Convert direction into object's local space
        Vector3 localDirToMouse = transform.InverseTransformDirection(worldDirToMouse);

        float verticalOffset = localDirToMouse.y;
        float tiltAngle = Mathf.Clamp(verticalOffset * _rotationSpeed, -_maxTiltAngle, _maxTiltAngle);

       
        rotationSpeed = _currentSwingAngle == 0 ? Mathf.Clamp(Vector3.Distance(worldMousePos, transform.position) * _rotationSpeed, 5f, 300f) 
            : swingSpeed;

        Quaternion targetRotation;

        if (_allowCameraTilt)
        {
            targetRotation = _currentSwingAngle == 0 ? Quaternion.Euler(-tiltAngle, 0f, 0f) : Quaternion.Euler(_currentSwingAngle, 0f, 0f);
        }
        else
        {
            targetRotation = Quaternion.Euler(_currentSwingAngle, 0f, 0f);
        }

        _panPivot.transform.localRotation = Quaternion.RotateTowards(
            _panPivot.transform.localRotation,
            targetRotation,
            Time.deltaTime * rotationSpeed);
    }

    private Vector3 FollowMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _originalDistance;

        Vector3 worldMousePos = _mainCam.ScreenToWorldPoint(mousePos);

        float moveSpeed = Mathf.Clamp(Vector3.Distance(worldMousePos, transform.position) * _moveSpeed, minSpeed, maxSpeed);

        if (Vector3.Distance(worldMousePos, transform.position) > _minDistanceToMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, worldMousePos.z), Time.deltaTime * moveSpeed);

            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, _originalPos.z - ClampPosZ, _originalPos.z + ClampPosZ));
        }
        return worldMousePos;
    }
}
