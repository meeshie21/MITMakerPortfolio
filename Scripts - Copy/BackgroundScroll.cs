using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public GameObject cylinder;
    public bool isScrolling = false;

    void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            float size = Random.Range(1f, 10f);

            GameObject go = Instantiate(cylinder, new Vector3(Random.Range(-40f, 40f), 0f, Random.Range(-15f, 15f)), Quaternion.identity);
            cylinder.transform.localScale = new Vector3(size, cylinder.transform.localScale.y, size);

            Destroy(go, 15f);
        }
    }

    void Update()
    {
      if (!isScrolling) StartCoroutine(BGScroll());
    }

    public IEnumerator BGScroll()
    {
        float size = Random.Range(1f, 10f);

        isScrolling = true;

        GameObject go = Instantiate(cylinder, new Vector3(Random.Range(-40f, 40f), 0f, 20f), Quaternion.identity) as GameObject;
        cylinder.transform.localScale = new Vector3(size, cylinder.transform.localScale.y, size);

        yield return new WaitForSeconds(0.25f);

        Destroy(go, 15f);

        isScrolling = false;
    }
}
