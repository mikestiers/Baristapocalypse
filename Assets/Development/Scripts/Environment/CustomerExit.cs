using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerExit : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CustomerBase>() != null)
        {
            if (other.GetComponent<CustomerBase>().GetCustomerState() == CustomerBase.CustomerState.Leaving) 
            {
                Destroy(other.gameObject);
            }
        }
    }
}
