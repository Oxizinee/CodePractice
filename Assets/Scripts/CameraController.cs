using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _moveSpeed = 3;
    private Vector3 _offset;
    void Start()
    {
        _offset = _player.transform.position - transform.position;
    }

    void LateUpdate()
    {
        transform.position = _player.transform.position - _offset;    
    }
}
