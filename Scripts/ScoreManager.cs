using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int score;
    public int previousScore;
    public int earlyScore;
    public List<int> frameScores;

    // Start is called before the first frame update
    void Start()
    {
        frameScores = new List<int>();
        earlyScore = 0;
        previousScore = 0;
        score = 0;
    }

    public int GetScore() { return this.score; }
    public int GetPreviousScore() { return this.previousScore; }
    public int GetEarlyScore() { return this.earlyScore; }

    public void SetScore(int score) { this.score = score; }
    public void SetPreviousScore(int previousScore) { this.previousScore = previousScore; }
    public void SetEarlyScore(int earlyScore) { this.earlyScore = earlyScore; }

    public void AddScoreToFrameList(int score) { frameScores.Add(score); }

    // Update is called once per frame
    void Update()
    {
        
    }
}
