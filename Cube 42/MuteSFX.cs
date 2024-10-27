using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteSFX : MonoBehaviour
{
    public GameObject audioManager;
    public Animator animator;

    void Start()
    {

    }

    void Update()
    {
        if (PlayerPrefs.GetInt("MutedSFX") == 0)
        {
            AudioListener.volume = 0f;
            animator.SetBool("isMuted", true);
        }

        else if (PlayerPrefs.GetInt("MutedSFX") == 1)
        {
            AudioListener.volume = 1f;
            animator.SetBool("isMuted", false);
        }
    }

    public void MuteSoundEffects()
    {
        if(PlayerPrefs.GetInt("MutedSFX") == 0)
        {
            PlayerPrefs.SetInt("MutedSFX", 1);
            animator.SetBool("isMuted", true);
            AudioListener.volume = 0f;
        }

        else if (PlayerPrefs.GetInt("MutedSFX") == 1)
        {
            PlayerPrefs.SetInt("MutedSFX", 0);
            animator.SetBool("isMuted", false);
            AudioListener.volume = 1f;
        }
    }
}
