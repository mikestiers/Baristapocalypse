using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WifiStation : RandomEventBase
{
    public Color Color = Color.white;
    public Color Color2 = Color.black;
    [SerializeField] private GameObject eventLight;
    [HideInInspector] NetworkVariable<bool> iseventover = new NetworkVariable<bool>(false);
    // Start is called before the first frame update
    void Start()
    {
      
    }
    public void WifiEventIsDone()
    {
        GameManager.Instance.isEventActive.Value = false;
        iseventover.Value = true;
        eventLight.SetActive(false);
        Debug.Log("Wifi event is done");
        ChangeColorBasedOnEvent();
    }

    public void WifiEventIsStarting() 
    {
        iseventover.Value = false;
        eventLight.SetActive(true);
        Debug.Log("Wifi event is Starting");
        ChangeColorBasedOnEvent();
    }

    void ChangeColorBasedOnEvent()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Material material = renderer.material;

            // Check the boolean condition
            if (iseventover.Value)
            {
                material.color = Color;
                Debug.Log("Changed color to white");
            }
            else
            {
                // Change the color of the material to the original color
                material.color = Color2;
                Debug.Log("Changed color to black");
            }
        }
        else
        {
            Debug.LogError("No Renderer component found on the GameObject.");
        }
    }
   
}
