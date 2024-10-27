using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text score;
    public TMP_Text[] rolls;
    public TMP_Text[] frames;
    public List<string> scores;
    public int rollNumber;
    public int totalScore;

    public ScoreManager scoreManager;
    public SpawnBall spawner;

    // Start is called before the first frame update
    void Start()
    {
        rollNumber = 0;
        totalScore = 0;
        scores = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "TOTAL: <b>" + totalScore.ToString();
        score.text = "TOTAL: <b>" + totalScore.ToString();
    }

    public void AddRollNumber() 
    {
        int earlyScore = scoreManager.GetEarlyScore();
        int previousScore = scoreManager.GetPreviousScore();
        int currentScore = scoreManager.GetScore();

        if (this.rollNumber == 19 && currentScore + previousScore < 10)
        {
            Debug.Log("DONE 1 Previous: " + previousScore + "|| Current: " + currentScore + " || Roll: " + rollNumber);
            int ind = IndexOfBlank(frames);
            totalScore += currentScore;
            frames[ind].text = totalScore.ToString();
            rolls[rollNumber].text = scoreManager.GetScore().ToString();
            rolls[rollNumber + 1].text = "-";
            spawner.SetDisable(true);
            return;
        }

        else if (this.rollNumber == 20)
        {
            Debug.Log("DONE 2 Previous: " + previousScore + "|| Current: " + currentScore + " || Roll: " + rollNumber);
            int ind = IndexOfBlank(frames);
            totalScore += (earlyScore + previousScore + currentScore);
            frames[ind].text = totalScore.ToString();
            if (currentScore == 10) rolls[rollNumber].text = "X";
            else rolls[rollNumber].text = (currentScore == 0) ? "-" : currentScore.ToString();
            spawner.SetDisable(true);
            return;
        }

        Debug.Log("Previous: " + previousScore + "|| Current: " + currentScore + " || Roll: " + rollNumber);

        if (rollNumber % 2 == 1 && currentScore + previousScore == 10)
        {
            rolls[rollNumber].text = "\\";
            scores.Add(rolls[rollNumber].text);
            if (scores.Count > 2 && scores[scores.Count - 3] == "X")
            {
                int ind = IndexOfBlank(frames);
                frames[ind].text = (totalScore + earlyScore + previousScore + currentScore).ToString();
                totalScore += (earlyScore + previousScore + currentScore);
                if (scores[scores.Count - 2] == "X" || scores[scores.Count - 1] == "X")
                {
                    this.rollNumber++;
                    return;
                }
                if (currentScore + previousScore != 10)
                {
                    frames[ind + 1].text = (totalScore + previousScore + currentScore).ToString();
                    totalScore += (previousScore + currentScore);
                }
            }
        }

        else if ((rollNumber % 2 == 0 || rollNumber >= 18) && currentScore == 10)
        {
            rolls[rollNumber].text = "X";
            scores.Add(rolls[rollNumber].text);
            if (this.rollNumber < 18) this.rollNumber++;
            if (scores.Count > 2 && scores[scores.Count - 3] == "X")
            {
                int ind = IndexOfBlank(frames);
                frames[ind].text = (totalScore + earlyScore + previousScore + currentScore).ToString();
                totalScore += (earlyScore + previousScore + currentScore);
                if (scores[scores.Count - 2] == "X" || scores[scores.Count - 1] == "X")
                {
                    this.rollNumber++;
                    return;
                }
                if (currentScore + previousScore != 10)
                {
                    frames[ind + 1].text = (totalScore + previousScore + currentScore).ToString();
                    totalScore += (previousScore + currentScore);
                }
            }
            else if (scores.Count > 1 && scores[scores.Count - 2] == "\\")
            { 
                frames[IndexOfBlank(frames)].text = (totalScore + 10 + currentScore).ToString();
                totalScore += (10 + currentScore);
            }
        }

        else if (currentScore == 0)
        {
            rolls[rollNumber].text = "-";
            scores.Add(rolls[rollNumber].text);
            if (scores.Count > 2 && scores[scores.Count - 3] == "X")
            {
                int ind = IndexOfBlank(frames);
                frames[ind].text = (totalScore + earlyScore + previousScore + currentScore).ToString();
                totalScore += (earlyScore + previousScore + currentScore);
                if (scores[scores.Count - 2] == "X" || scores[scores.Count - 1] == "X")
                {
                    this.rollNumber++;
                    return;
                }
                if (currentScore + previousScore != 10)
                {
                    frames[ind + 1].text = (totalScore + previousScore + currentScore).ToString();
                    totalScore += (previousScore + currentScore);
                }
            }
            else if (scores.Count > 1 && scores[scores.Count - 2] == "\\")
            {
                frames[IndexOfBlank(frames)].text = (totalScore + 10 + currentScore).ToString();
                totalScore += (10 + currentScore);
            }
            else if (rollNumber % 2 == 1)
            {
                frames[IndexOfBlank(frames)].text = (totalScore + previousScore + currentScore).ToString();
                totalScore += (previousScore + currentScore);
            }
        }

        else
        {
            rolls[rollNumber].text = scoreManager.GetScore().ToString();
            scores.Add(rolls[rollNumber].text);
            if (scores.Count > 2 && scores[scores.Count - 3] == "X")
            {
                int ind = IndexOfBlank(frames);
                frames[ind].text = (totalScore + earlyScore + previousScore + currentScore).ToString();
                totalScore += (earlyScore + previousScore + currentScore);
                if (scores[scores.Count - 2] == "X" || scores[scores.Count - 1] == "X")
                {
                    this.rollNumber++;
                    return;
                }
                if (currentScore + previousScore != 10)
                {
                    frames[ind + 1].text = (totalScore + previousScore + currentScore).ToString();
                    totalScore += (previousScore + currentScore);
                }
            }
            else if (scores.Count > 1 && scores[scores.Count - 2] == "\\")
            {
                frames[IndexOfBlank(frames)].text = (totalScore + 10 + currentScore).ToString();
                totalScore += (10 + currentScore);
            }
            else if (rollNumber % 2 == 1)
            {
                frames[IndexOfBlank(frames)].text = (totalScore + previousScore + currentScore).ToString();
                totalScore += (previousScore + currentScore);
            }
        }

        this.rollNumber++;
    }

    public int IndexOfBlank(TMP_Text[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].text.Equals("")) return i;
        }

        return 0;
    }
}
