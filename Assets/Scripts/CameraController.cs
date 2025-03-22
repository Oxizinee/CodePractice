using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _moveSpeed = 3;
    private Vector3 _followPosition;
    void Start()
    {
        _followPosition = _player.transform.position - transform.position;
    }

    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, _followPosition, Time.deltaTime * _moveSpeed);
    }
}
