using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool hasCollided;
    private Rigidbody rb;
    private static float forceFactor = 10f;

    public bool HasCollided() { return this.hasCollided; }

    // Start is called before the first frame update
    void Start()
    {
        hasCollided = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(Vector3.forward * 100f);
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "Pin" || collisionInfo.collider.tag == "PinCollider")
        {
            this.hasCollided = true;

            Rigidbody rb = collisionInfo.collider.tag == "Pin"
                    ? collisionInfo.collider.GetComponent<Rigidbody>()
                    : collisionInfo.collider.transform.parent.GetComponent<Rigidbody>();

            rb.AddForce(collisionInfo.contacts[0].normal * forceFactor);
        }
    }
}
