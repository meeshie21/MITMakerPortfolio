using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public float swayForce = 0.1f;
    public Vector3[] randPositions;

    private float minX = -3f;
    private float maxX = 3f;
    private float minY = 1f;
    private float maxY = 3f;
    private float minZ = -12f;
    private float maxZ = -8f;
    private float errorMargin = 0.5f;
    private float waitTime = 0.35f;
    private Vector3 randPos;

    // Start is called before the first frame update
    void Start()
    {
        randPos = randPositions[Random.Range(0, randPositions.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, randPos) >= errorMargin) transform.position = Vector3.Lerp(transform.position, randPos, swayForce);
        else randPos = randPositions[Random.Range(0, randPositions.Length)];
    }
}
