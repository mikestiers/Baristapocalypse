using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 2f;

    void Update()
    {

    }

    public void PlaySceneTransition()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex <= 1)
        {
            StartCoroutine(SceneTransition(sceneIndex + 1));
        }
        else if (sceneIndex == 2)
        {
            // Add game over return to lobby/main menu functionality
            return;
        }
        else if (sceneIndex == 3)
        {
            StartCoroutine(SceneTransition(sceneIndex));
        }
    }

    IEnumerator SceneTransition(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        if (levelIndex != 3)
        {
            SceneManager.LoadScene(levelIndex);
        }
    }
}