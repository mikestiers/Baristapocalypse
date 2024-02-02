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

        if (sceneIndex != 2)
        {
            StartCoroutine(SceneTransition(sceneIndex + 1));
        }
        else if (sceneIndex == 2)
        {
            return;
        }
    }

    IEnumerator SceneTransition(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
