using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScroller : MonoBehaviour
{
    public float scrollSpeed = 2.5f;
    public TMPro.TextMeshProUGUI textComponent;
    Vector3 originalCreditsTextTransformPosition;
    private bool isMoving;

    private void Start()
    {
        originalCreditsTextTransformPosition = textComponent.transform.position;
    }

    void Update()
    {
        if (textComponent != null)
        {
            if (isMoving)
            {
                MoveText();
            }
            else
            {
                ResetText();
            }
        }
    }

    public void MoveText()
    {
        isMoving = true;
        Vector3 pos = textComponent.transform.position;
        pos.y += scrollSpeed * Time.deltaTime;
        textComponent.transform.position = pos;
    }
    public void ResetText()
    {
        Debug.Log("we here?");
        isMoving = false;
        textComponent.transform.position = originalCreditsTextTransformPosition;
    }
}
