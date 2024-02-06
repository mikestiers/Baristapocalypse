using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTING_MinigameCollider : MonoBehaviour
{
    [SerializeField] private GameObject minigameUIObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            minigameUIObject.SetActive(true);
            Debug.Log("Minigame object activated");
        }
    }
}
