using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public bool isRunning = false;

    public Text tapToStart;

    public Text difficultyUI;
    public float difficulty;
    public float a;

    public int coinGain;

    public AudioSource audioSource;
    public PauseOnStart pos;

    void Start()
    {
        a = 1f;

        tapToStart.gameObject.SetActive(true);

        Time.timeScale = 1f;
        coinGain = 0;
    }

    void Update()
    {
        if (!pos.hasStarted) return;

        tapToStart.gameObject.SetActive(false);

        if (!isRunning) StartCoroutine(ObstacleSpeed());
    }

    public void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "Obstacle")
        {
            Time.timeScale = 0.0001f;
            Invoke("LoadGameOver", 0.0001f);
        }

        if (collisionInfo.collider.tag == "Coin")
        {
            coinGain++;
            PlayerPrefs.SetInt("CoinGain", coinGain);
            PlayerPrefs.SetInt("CoinTotal", PlayerPrefs.GetInt("CoinTotal") + 1);

            audioSource.Play();
            Destroy(collisionInfo.collider.gameObject);
        }
    }

    public void LoadGameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }

    public IEnumerator ObstacleSpeed()
    {
        isRunning = true;

        yield return new WaitForSeconds(2.25f);

        Time.timeScale += 0.01f;
        a += 1f;

        difficulty = a;
        difficultyUI.text = "Speed: " + difficulty + "mph";

        PlayerPrefs.SetFloat("Difficulty", difficulty);

        Debug.Log(Time.timeScale);

        isRunning = false;
    }
}
