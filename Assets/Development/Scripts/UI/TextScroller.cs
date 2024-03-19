using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScroller : MonoBehaviour
{
    public float scrollSpeed = 2.5f;
    public GameObject creditsContainer;
    Vector3 originalCreditsTextTransformPosition;
    private bool isMoving;

    private void Start()
    {
        originalCreditsTextTransformPosition = creditsContainer.transform.position;
    }

    void Update()
    {
        if (creditsContainer != null)
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
        Vector3 pos = creditsContainer.transform.position;
        pos.y += scrollSpeed * Time.deltaTime;
        creditsContainer.transform.position = pos;
    }
    public void ResetText()
    {
        Debug.Log("we here?");
        isMoving = false;
        creditsContainer.transform.position = originalCreditsTextTransformPosition;
    }
}
