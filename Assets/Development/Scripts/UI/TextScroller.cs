using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScroller : MonoBehaviour
{
    public float scrollSpeed = 2.5f;
    public TMPro.TextMeshProUGUI textComponent;
    void Update()
    {
        if (textComponent != null)
        {
            Vector3 pos = textComponent.transform.position;
            pos.y += scrollSpeed * Time.deltaTime;
            textComponent.transform.position = pos;
        }
    }
}
