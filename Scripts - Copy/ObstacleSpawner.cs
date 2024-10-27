using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstacles;
    public bool isScrolling = false;

    public PauseOnStart pos;

    void Start()
    {
        if (!pos.hasStarted) return;

        StartCoroutine(Spawn());
    }

    void Update()
    {
        if (!pos.hasStarted) return;

        if (!isScrolling) StartCoroutine(Spawn());
    }

    public IEnumerator Spawn()
    {
        isScrolling = true;

        yield return new WaitForSeconds(0.6f);

        GameObject go = Instantiate(obstacles[Random.Range(1, obstacles.Length)], new Vector3(7.5f, 0f, 20f), Quaternion.identity);

        Destroy(go, 15f);

        isScrolling = false;
    }
}
