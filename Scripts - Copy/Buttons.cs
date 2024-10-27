using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public AudioSource audioSource;

    public void LoadOblivion()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadEuphoria()
    {
        SceneManager.LoadScene("Game 2");
    }

    public void LoadAether()
    {
        SceneManager.LoadScene("Game 3");
    }

    public void LoadElysium()
    {
        SceneManager.LoadScene("Game 4");
    }

    public void LoadModeSelect()
    {
        SceneManager.LoadScene("Mode Select");
    }

    public void LoadMapSelect()
    {
        SceneManager.LoadScene("Map Select");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void LoadShop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void ButtonPress()
    {
        audioSource.Play();
        Invoke("LoadOblivion", 0.2f);
    }

    public void PlayAudio()
    {
        audioSource.Play();
    }
}
