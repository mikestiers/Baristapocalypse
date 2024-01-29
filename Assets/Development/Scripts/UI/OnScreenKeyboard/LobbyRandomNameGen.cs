using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyRandomNameGen : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyName;
    [SerializeField] private TMP_Text resultText;

    private string RoomName;
    //clear whats in inputfield first
    // check lobby list for same name in use
    // if not use name/ if so prompt new name
    // create lobby name

    public void createName()
    {
        int roomNumber = Random.Range(1, 101);

        string randomName = $"Room:{roomNumber}";

        resultText.text = randomName;
    }
    
}
