using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseOnStart : MonoBehaviour
{
    public bool hasStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        hasStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            hasStarted = true;
        }
    }
}
