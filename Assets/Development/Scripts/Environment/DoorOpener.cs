using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private Animator leftDoorAnimator;
    [SerializeField] private Animator rightDoorAnimator;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Customer"))
        {
            leftDoorAnimator.SetBool("isOpen", true);
            rightDoorAnimator.SetBool("isOpen", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Customer"))
        {
            leftDoorAnimator.SetBool("isOpen", false);
            rightDoorAnimator.SetBool("isOpen", false);
        }
    }
}
