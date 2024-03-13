using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectUI : MonoBehaviour
{
    public int colorId;
    public Image image;
    public GameObject selectedGameobject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            BaristapocalypseMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        BaristapocalypseMultiplayer.Instance.OnPlayerDataNetworkListChanged += BaristapocalypseMultiplayer_OnPlayerDataNetworkListChanged;  
        image.color = BaristapocalypseMultiplayer.Instance.GetPlayerColor(colorId).color;
        UpdataIsSelected();
    }

    private void BaristapocalypseMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdataIsSelected();
    }

    private void UpdataIsSelected()
    {
        if (BaristapocalypseMultiplayer.Instance.GetPlayerData().colorId == colorId)
        {
            selectedGameobject.SetActive(true);
        }

        else
        {
            selectedGameobject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        BaristapocalypseMultiplayer.Instance.OnPlayerDataNetworkListChanged -= BaristapocalypseMultiplayer_OnPlayerDataNetworkListChanged;
    }
}
