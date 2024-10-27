using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public bool isBallTrigger;
    public UIManager manager;
    public ScoreManager score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collisionInfo)
    {
        /*if(collisionInfo.collider.tag == "Pin" || collisionInfo.collider.tag == "PinCollider" && score.GetScore() <= 10) 
        {
            Transform col = collisionInfo.collider.tag == "Pin" ? collisionInfo.collider.transform : collisionInfo.collider.transform.parent;
            Destroy(col.gameObject);
            score.SetScore(score.GetScore() + 1);
        }*/

        if(collisionInfo.collider.tag == "Ball" && !isBallTrigger)
        {
            StartCoroutine(EnableThenDisable(collisionInfo.collider));
        }

        if(collisionInfo.collider.tag == "Ball" && isBallTrigger)
        {
            if((GameObject.FindObjectsOfType(typeof(Pin)) as Pin[]).Length > 0) 
            {
                foreach (Pin p in GameObject.FindObjectsOfType(typeof(Pin)) as Pin[])
                {
                    if(p.IsDone() && score.GetScore() <= 10) //TEMPORARY SOLUTION???
                    {
                        Destroy(p.gameObject);
                        score.SetScore(score.GetScore() + 1);
                    }
                }
            }

            Destroy(collisionInfo.collider.transform.gameObject);
            manager.AddRollNumber();
            score.SetEarlyScore(score.GetPreviousScore());
            score.SetPreviousScore(score.GetScore());
            score.SetScore(0);
        }

    }

    public List<Rigidbody> movingRigidbodies(Collision collisionInfo)
    {
        List<Rigidbody> returnList = new List<Rigidbody>();
        Rigidbody[] allRigidbodies = GameObject.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];

        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb.velocity.magnitude > 0 && rb != collisionInfo.collider.GetComponent<Rigidbody>())
            {
                returnList.Add(rb);
            }        
        }

        return returnList;
    }

    public IEnumerator EnableThenDisable(Collider col)
    {
        col.enabled = false;
        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
        StopAllCoroutines();
    }
}
    
