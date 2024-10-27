using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Prefs;

public class Stopwatch : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private ProtectedFloat seconds;

    // Start is called before the first frame update
    void Start()
    {
        seconds = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.started && !Player.gameOver) seconds += Time.deltaTime;
        if (Player.started) text.color = Color.black;
        if (Player.started) text.text = TimeText(seconds);
        else text.text = "Tap anywhere to start.";
        GetComponent<Animation>().enabled = (!Player.started);
    }

    public float GetSeconds()
    {
        if (this.seconds != null) return this.seconds;
        else return 0f;
    }

    public static string TimeText(float seconds)
    {
        string output = "";
        output += ((int)(seconds / 60f)).ToString("D2");
        output += ":";
        output += ((int)seconds % 60).ToString("D2");
        return output;
    }
}
