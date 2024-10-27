using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectiveParent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, transform.childCount);
        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(i == rand);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
