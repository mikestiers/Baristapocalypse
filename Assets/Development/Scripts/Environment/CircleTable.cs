using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircleTable : MonoBehaviour
{
    [SerializeField] private GameObject centerofTable;
    private const float CrossFadeDuration = 0.1f;
    private readonly int Customer_SitDownHash = Animator.StringToHash("Customer_SitDown");

    private void OnTriggerEnter(Collider other)
    {
       
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<CustomerBase>() != null)
        {
            CustomerBase customer = other.gameObject.GetComponent<CustomerBase>();

            if (customer.atSit == true)
            {
                customer.customerAnimator.CrossFadeInFixedTime(Customer_SitDownHash, CrossFadeDuration);
                other.gameObject.transform.LookAt(centerofTable.transform.position);
            }
               
        }
    }
}
