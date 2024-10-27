using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Ball ball;
    [SerializeField] private GameObject[] courses;
    public int courseIndex;

    void Awake()
    {
        courseIndex = Random.Range(0, courses.Length);
        GameObject course = courses[courseIndex];
        Instantiate(course, course.transform.position, course.transform.rotation);
        GameObject.FindObjectOfType<UIManager>().Initialize();
    }

    void Start()
    {
        ball = GameObject.FindObjectOfType<Ball>().GetComponent<Ball>();
    }

    void Update()
    {
        if(ball.ReachedHole())
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        GameObject.FindObjectOfType<UIManager>().EndGameUI();
        PlayerPrefs.SetInt("ShotsTaken", PlayerPrefs.GetInt("ShotsTaken") + ball.GetShotsTaken());
        PlayerPrefs.SetInt("Par", PlayerPrefs.GetInt("Par") + GameObject.FindObjectOfType<Course>().GetComponent<Course>().par);
    }
}
