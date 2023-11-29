using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDebug : Singleton<UIDebug>
{
    public TextMeshProUGUI UIDebugText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDebugMessage(UIDebugText.text);
    }

    public void UpdateDebugMessage(string text)
    {
        UIDebugText.text = text;
    }
}
