using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBall : MonoBehaviour
{
    public int currentTurn;
    public bool ableToReset;
    public bool disabled;

    public GameObject ball;
    public GameObject spawnBall;
    public Swipe swipe;

    public float spawnFactor;
    public float lastTurn;
    public int lastLength;

    public float minForceFactor;
    public float forceFactor;
    public float torqueFactor;

    public bool alreadySpawned;
    private Vector3 positionToSpawn;

    [SerializeField] private float turnMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        alreadySpawned = false;
        ableToReset = false;
        disabled = false;
        currentTurn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentTurn);
        ableToReset = (currentTurn >= 2);

        if (!disabled && GameObject.Find("Ball(Clone)") == null && Input.touchCount > 0 /*&& !alreadySpawned*/ && Input.GetTouch(0).phase == TouchPhase.Ended && !ableToReset)
        {
            if (Input.GetTouch(0).rawPosition.y - Input.GetTouch(0).position.y >= 0) return;
            alreadySpawned = true;

            Touch touch = Input.GetTouch(0);

            InstantiateBall(touch);
            AddBallForce(touch);
            currentTurn++;
        }
    }

    public void AddBallForce(Touch touch)
    {
        spawnBall = GameObject.Find("Ball(Clone)");

        lastLength = swipe.DetectSwipe(touch).length;
        lastTurn = swipe.DetectSwipe(touch).turnFactor * turnMultiplier;

        if (spawnBall != null)
        {
            spawnBall.GetComponent<Rigidbody>().AddForce(lastTurn * forceFactor * -0.075f, 0f, Mathf.Max(minForceFactor, forceFactor * lastLength));
        }
    }

    void InstantiateBall(Touch t)
    {
        float halfScreen = (Screen.width / 2f);
        float ratio = 0f;
      

        if(t.rawPosition.x < halfScreen)
        {
            Debug.Log("left side bowl");
            ratio = t.rawPosition.x / halfScreen;
            Debug.Log("Touch Position: " + t.rawPosition.x + " || Half Screen: " + halfScreen);
            Instantiate(ball, new Vector3((ratio * -1f * 2.5f), -3.5f, 2f), Quaternion.identity);
        }

        else
        {
            Debug.Log("right side bowl");
            ratio = ((float)Mathf.Abs(halfScreen - t.rawPosition.x))/halfScreen;
            Instantiate(ball, new Vector3((ratio * 2.5f), -3.5f, 2f), Quaternion.identity);
        }

    }

    public void SetDisable(bool disabled) { this.disabled = disabled; }
    public bool IsAbleToReset() { return this.ableToReset && GameObject.Find("Ball(Clone)") == null; }
    public int GetCurrentTurn() { return this.currentTurn; }
    public void SetCurrentTurn(int currentTurn) { this.currentTurn = currentTurn; }
}
