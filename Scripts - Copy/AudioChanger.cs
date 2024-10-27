using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChanger : MonoBehaviour
{
    public AudioSource source1;
    public AudioSource source2;
    public AudioSource source3;
    public AudioSource source4;

    void Start()
    {

    }

    void Update()
    {
        if (PlayerPrefs.GetInt("AudioValue") == 1)
        {
            source1.gameObject.SetActive(true);
            source2.gameObject.SetActive(false);
            source3.gameObject.SetActive(false);
            source4.gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt("AudioValue") == 2)
        {
            source1.gameObject.SetActive(false);
            source2.gameObject.SetActive(true);
            source3.gameObject.SetActive(false);
            source4.gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt("AudioValue") == 3)
        {
            source1.gameObject.SetActive(false);
            source2.gameObject.SetActive(false);
            source3.gameObject.SetActive(true);
            source4.gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt("AudioValue") == 4)
        {
            source1.gameObject.SetActive(false);
            source2.gameObject.SetActive(false);
            source3.gameObject.SetActive(false);
            source4.gameObject.SetActive(true);
        }
    }
}
