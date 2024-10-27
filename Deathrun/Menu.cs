using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Prefs;

public class Menu : MonoBehaviour
{
    [SerializeField] private TMP_Text xp, tier, rank;
    [SerializeField] private Fade fade;

    // Start is called before the first frame update
    void Start()
    {
        /*ProtectedPlayerPrefs.SetInt("xp", 0);
        ProtectedPlayerPrefs.SetInt("tier", 0);
        ProtectedPlayerPrefs.SetInt("rank", 0);
        ProtectedPlayerPrefs.SetInt("starterHighScore", 0);
        ProtectedPlayerPrefs.SetInt("easyHighScore", 0);
        ProtectedPlayerPrefs.SetInt("normalHighScore", 0);
        ProtectedPlayerPrefs.SetInt("hardHighScore", 0);
        ProtectedPlayerPrefs.SetInt("deathHighScore", 0);
        PlayerPrefs.SetString("tutorial", "no");*/

        if (PlayerPrefs.GetString("tutorial") != "done")
        {
            fade.LoadScene(3);
        }

        xp.text = ProtectedPlayerPrefs.GetInt("xp").ToString();
        tier.text = ProtectedPlayerPrefs.GetInt("tier").ToString();

        int ranking = ProtectedPlayerPrefs.GetInt("rank");
        if (ranking == 0) rank.text = "N/A";
        else rank.text = (ranking >= 1) ? ranking.ToString() : "N/A";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
