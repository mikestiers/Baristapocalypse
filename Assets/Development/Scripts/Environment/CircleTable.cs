using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircleTable : MonoBehaviour
{
    [SerializeField] private GameObject centerofTable;

    private void OnTriggerEnter(Collider other)
    {
       
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<CustomerBase>() != null)
        {
            CustomerBase customer = other.gameObject.GetComponent<CustomerBase>();

            if (customer.atSit == true) other.gameObject.transform.LookAt(centerofTable.transform.position);      
               
        }
    }
}
