using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameras : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;
    public GameObject object4;

    void Update()
    {
        // Check for keyboard input to toggle objects
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ToggleObject(object1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ToggleObject(object2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ToggleObject(object3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ToggleObject(object4);
        }
    }

    // Function to toggle the visibility of a single object and turn off others
    void ToggleObject(GameObject targetObject)
    {
        object1.SetActive(targetObject == object1);
        object2.SetActive(targetObject == object2);
        object3.SetActive(targetObject == object3);
        object4.SetActive(targetObject == object4);
    }
}
