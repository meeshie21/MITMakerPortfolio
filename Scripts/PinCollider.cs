using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinCollider : MonoBehaviour
{
    public bool isDone;

    // Start is called before the first frame update
    void OnEnable()
    {
        isDone = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
       
    }
}
