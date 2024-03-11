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
    public delegate void WifiEventHandler();
    public static event WifiEventHandler OnWifiEventStarting;
    public static event WifiEventHandler OnWifiEventStopping;

    
    [ServerRpc(RequireOwnership = false)]
    public void WifiEventIsDoneServerRpc()
    {
        WifiEventIsDoneClientRpc();
    }

    [ClientRpc]
    private void WifiEventIsDoneClientRpc()
    {
        WifiEventIsDone();
    }

    private void WifiEventIsDone()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.wifiOn);
        GameManager.Instance.isEventActive.Value = false;
        GameManager.Instance.isWifiEvent.Value = false;
        iseventover.Value = true;
        screenEffect.ToggleWifiEffect(iseventover.Value);
        eventLight.SetActive(false);
        Debug.Log("Wifi event is done");
        ChangeColorBasedOnEvent();
        OnWifiEventStopping?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void WifiEventIsStartingServerRpc()
    {
        WifiEventIsStartingClientRpc();
    }

    [ClientRpc]
    private void WifiEventIsStartingClientRpc()
    {
        WifiEventIsStarting();
    }

    private void WifiEventIsStarting() 
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.wifiOff);
        iseventover.Value = false;
        screenEffect.ToggleWifiEffect(iseventover.Value);
        GameManager.Instance.isWifiEvent.Value = true;
        eventLight.SetActive(true);
        Debug.Log("Wifi event is Starting");
        ChangeColorBasedOnEvent();
        OnWifiEventStarting?.Invoke();
    }

    private void ChangeColorBasedOnEvent()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Material material = renderer.material;

            // Check the boolean condition
            if (iseventover.Value)
            {
                material.color = Color2;
                Debug.Log("Changed color to white");
            }
            else
            {
                // Change the color of the material to the original color
                material.color = Color;
                Debug.Log("Changed color to black");
            }
        }
        else
        {
            Debug.LogError("No Renderer component found on the GameObject.");
        }
    }
   
}
