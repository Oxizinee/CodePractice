using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public GameObject CustomerPrefab;
    [SerializeField]private GameObject _currentCustomer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && _currentCustomer == null)
        {
            _currentCustomer = Instantiate(CustomerPrefab, transform.position, Quaternion.identity,transform);
            _currentCustomer.GetComponent<Customer>().OnWalkAway += ClearCurrentCustomer;         
        }
    }

    private void ClearCurrentCustomer()
    {
        _currentCustomer = null;
    }
}
