using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private Image starsFill;
    private int par;
    private int shotsTaken;

    // Start is called before the first frame update
    void Start()
    {
        shotsTaken = PlayerPrefs.GetInt("ShotsTaken");
        par = PlayerPrefs.GetInt("Par");
        starsFill.fillAmount = CalcFillAmount(par, shotsTaken); //ADD CALCULATION METHOD SOMEWHERE
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static float CalcFillAmount(int par, int shotsTaken)
    {
        int diff = shotsTaken - par;

        if (diff > 0) return 0.6f * 1f / (float)diff;
        else if (diff < 0) return -1f / (float)diff;
        else return 0.71f;
    }
}
