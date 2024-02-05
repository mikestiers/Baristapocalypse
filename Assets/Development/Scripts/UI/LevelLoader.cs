using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 2f;

    private void Start()
    {
        transition.Play("SceneTransition_End");
    }

    public void PlaySceneTransition()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex <= 1)
        {
            StartCoroutine(SceneTransition(sceneIndex + 1));
        }
        else
        {
            StartCoroutine(SceneTransition(sceneIndex));
        }
    }

    IEnumerator SceneTransition(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            // DOES NOT WORK, Breaks object references in UIManager.cs line 167-172
            //SceneManager.LoadScene(0);
            //SceneHelper.Instance.ResetGame();
        }
        else if (levelIndex < 3)
        {
            SceneManager.LoadScene(levelIndex);
        }
    }
}