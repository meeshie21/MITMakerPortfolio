using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "PinCollider")
        {
            Destroy(collisionInfo.collider.transform.parent.gameObject);
            ScoreManager score = GameObject.FindObjectOfType<ScoreManager>().GetComponent<ScoreManager>();
            score.SetScore(score.GetScore() + 1);
        }
    }*/
}
