using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarChairs : MonoBehaviour
{
    [SerializeField] private GameObject chairForward;
    [SerializeField] private GameObject scoochPoint;
    private const float CrossFadeDuration = 0.1f;
    private readonly int Customer_SitDownHash = Animator.StringToHash("Customer_SitDown");

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<CustomerBase>() != null)
        {
            CustomerBase customer = other.gameObject.GetComponent<CustomerBase>();
            if (customer.atSit == true)
            {
                customer.customerAnimator.CrossFadeInFixedTime(Customer_SitDownHash, CrossFadeDuration);
                other.gameObject.transform.LookAt(chairForward.transform.position);
            }



           // if (scoochPoint)


        }
    }

}
