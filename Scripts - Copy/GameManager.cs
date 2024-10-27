using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] meshes;

    void Start()
    {
       
    }

    void Update()
    {
        int meshNumber = PlayerPrefs.GetInt("MeshNumber");
        meshes[meshNumber].SetActive(true);
    }
}
