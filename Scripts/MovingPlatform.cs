using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public static float speed = 5f;
    public bool isConnected;
    public GameObject connectedPlatform;
    public int difficultyLevel;

    // Start is called before the first frame update
    void Awake()
    {
        speed = 5f;

        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.tag == "Floor") transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(-1f * speed * Time.deltaTime, 0f, 0f));
    }
}
