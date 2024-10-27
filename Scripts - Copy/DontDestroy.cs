using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Start()
    {
        GameObject[] audioSources = GameObject.FindGameObjectsWithTag("Music");
        
        if(audioSources.Length >  1)
        {
            Destroy(this.gameObject);
        }

        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
