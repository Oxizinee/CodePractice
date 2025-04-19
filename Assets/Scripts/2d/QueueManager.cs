using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public GameObject CustomerPrefab;
    [SerializeField]private GameObject _currentCustomer;
    DayManager dayManager;
    private void Awake()
    {
        dayManager = FindObjectOfType<DayManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && _currentCustomer == null)
        {
            _currentCustomer = Instantiate(CustomerPrefab, transform.position, Quaternion.identity,transform);
            _currentCustomer.GetComponent<Customer>().SetCurrentDayRules(dayManager.currentDayConfig.rules);
            _currentCustomer.GetComponent<Customer>().OnWalkAway += ClearCurrentCustomer;
            Debug.Log("New Customer incoming");
        }
    }

    private void ClearCurrentCustomer()
    {
        _currentCustomer = null;
    }
}
