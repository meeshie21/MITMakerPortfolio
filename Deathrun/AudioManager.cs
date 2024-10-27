using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static GameObject instance;

    void Awake()
    {
        if (instance == null) instance = this.gameObject;
        else Destroy(this.gameObject);
        DontDestroyOnLoad(this);
    }
}
