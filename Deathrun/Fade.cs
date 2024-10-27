using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    [SerializeField] public Animator animator;
    private int levelToLoad;

    void Update()
    {
        
    }

    public void LoadScene(int sceneIndex)
    {
        animator.SetTrigger("Fade Out");
        levelToLoad = sceneIndex;
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
