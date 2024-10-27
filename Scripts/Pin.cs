using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    private float forceFactor = 3f;
    private float errorMargin = 1f;
    private float startXRot;
    private float startZRot;

    public bool IsDone()
    {
        float xDiff = Mathf.Abs(transform.eulerAngles.x - startXRot);
        float zDiff = Mathf.Abs(transform.eulerAngles.z - startZRot);
        Debug.Log("XDiff: " + xDiff + " || ZDiff: " + zDiff);
        return xDiff >= errorMargin || zDiff >= errorMargin;

        //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
    }

    void OnEnable()
    {
        startXRot = transform.eulerAngles.x;
        startZRot = transform.eulerAngles.z;
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
       
    }
}
