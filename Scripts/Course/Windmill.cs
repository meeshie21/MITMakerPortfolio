using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : MonoBehaviour
{
    [SerializeField] private Transform t;
    [SerializeField] private float rotSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t.Rotate(0f, 0f, 0.1f * rotSpeed, Space.Self);

    }
}
