using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarChairs : MonoBehaviour
{
    [SerializeField] private GameObject chairForward;
    [SerializeField] private GameObject scoochPoint;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<CustomerBase>() != null)
        {
            CustomerBase customer = other.gameObject.GetComponent<CustomerBase>();
            if (scoochPoint)
            {
                customer.isSittingAtBooth.Value = true; 
            }
            if (customer.atSit == true) other.gameObject.transform.LookAt(chairForward.transform.position);

        }
    }

}
