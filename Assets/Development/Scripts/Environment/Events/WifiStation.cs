using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WifiStation : RandomEventBase
{
    public Color Color = Color.white;
    public Color Color2 = Color.black;
    [SerializeField] private GameObject eventLight;
    [SerializeField] private NetworkVariable<bool> iseventover = new NetworkVariable<bool>(false);
    public delegate void WifiEventHandler(bool isWifiOn);
    public static event WifiEventHandler OnWifiEventStarting;
    public static event WifiEventHandler OnWifiEventStopping;

    protected override void Start()
    {
        base.Start();
    }
    public void WifiEventIsDone()
    {
        WifiEventIsDoneServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void WifiEventIsDoneServerRpc()
    {
        GameManager.Instance.isEventActive.Value = false;
        GameManager.Instance.isWifiEvent.Value = false;
        iseventover.Value = true;
        WifiEventIsDoneClientRpc();  
    }

    [ClientRpc]
    private void WifiEventIsDoneClientRpc()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.wifiOn);
        screenEffect.ToggleWifiEffect(iseventover.Value);
        //fx.StartnEndFireworks();
        eventLight.SetActive(false);
        Debug.Log("Wifi event is done");
        ChangeColorBasedOnEvent(true);
        OnWifiEventStopping?.Invoke(false);
    }

    public void WifiEventIsStarting()
    {
        WifiEventIsStartingServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void WifiEventIsStartingServerRpc()
    {
        iseventover.Value = false;
        WifiEventIsStartingClientRpc();
    }

    [ClientRpc]
    private void WifiEventIsStartingClientRpc()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.wifiOff);
        screenEffect.ToggleWifiEffect(iseventover.Value);
        eventLight.SetActive(true);
        Debug.Log("Wifi event is Starting");
        ChangeColorBasedOnEvent(false);
        OnWifiEventStarting?.Invoke(true);
    }

    

    private void ChangeColorBasedOnEvent(bool eventOver)
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Material material = renderer.material;

            // Check the boolean condition
            if (eventOver)
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
