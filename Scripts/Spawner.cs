using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] floors;
    [SerializeField] private GameObject[] randomizedFloors;
    [SerializeField] private GameObject[] roofs;

    //[SerializeField] private GameObject[] randomizedRoofs;

    private bool connectNextFloor;
    private GameObject nextFloor;

    private bool connectNextRoof;
    private GameObject nextRoof;

    private int gameDifficultyLevel;

    private List<GameObject> easyFloors, normalFloors, hardFloors;
    private List<GameObject> easyRoofs, normalRoofs, hardRoofs;
    [SerializeField] private List<GameObject> easyRandomFloors;
    [SerializeField] private List<GameObject> starterFloors;
    [SerializeField] private List<GameObject> deathFloors, deathRoofs;

    private int startPoint = 2;

    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.SetInt("difficultyLevel", 2);
        gameDifficultyLevel = PlayerPrefs.GetInt("difficultyLevel");

        connectNextFloor = false;
        connectNextRoof = false;

        easyFloors = new List<GameObject>();
        normalFloors = new List<GameObject>();
        hardFloors = new List<GameObject>();
        easyRoofs = new List<GameObject>();
        normalRoofs = new List<GameObject>();
        hardRoofs = new List<GameObject>();
        starterFloors = new List<GameObject>();
        deathFloors = new List<GameObject>();
        deathRoofs = new List<GameObject>();

        foreach (GameObject floor in floors)
        {
            int difficulty = floor.GetComponent<MovingPlatform>().difficultyLevel;

            switch(difficulty)
            {
                case -1:
                    starterFloors.Add(floor);
                    easyFloors.Add(floor);
                    break;
                case 0:
                    easyFloors.Add(floor);
                    normalFloors.Add(floor);
                    break;
                case 1:
                    normalFloors.Add(floor);
                    hardFloors.Add(floor);
                    break;
                case 2:
                    hardFloors.Add(floor);
                    deathFloors.Add(floor);
                    break;
                case 3:
                    deathFloors.Add(floor);
                    break;
            }
        }

        foreach (GameObject roof in roofs)
        {
            int difficulty = roof.GetComponent<MovingPlatform>().difficultyLevel;

            switch (difficulty)
            {
                case 0:
                    easyRoofs.Add(roof);
                    normalRoofs.Add(roof);
                    break;
                case 1:
                    normalRoofs.Add(roof);
                    hardRoofs.Add(roof);
                    break;
                case 2:
                    hardRoofs.Add(roof);
                    deathRoofs.Add(roof);
                    break;
                case 3:
                    deathRoofs.Add(roof);
                    break;
            }
        }

        for (int i = startPoint; i < 7; i++)
        {
            SpawnFloors(i * 20, gameDifficultyLevel);
            if(gameDifficultyLevel != -1) SpawnRoofs(i * 20, gameDifficultyLevel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnFloors(float xPos, int difficulty)
    {
        GameObject platform = null;

        if (connectNextFloor) platform = nextFloor;
        else switch(difficulty)
        {
            case -1:
                platform = starterFloors[Random.Range(0, starterFloors.Count)];
                break;
            case 0:
                platform = (Random.Range(0, 2) == 1) 
                    ? easyFloors[Random.Range(0, easyFloors.Count)] : easyRandomFloors[Random.Range(0, easyRandomFloors.Count)];
                break;
            case 1:
                platform = (Random.Range(0, 2) == 1)
                    ? normalFloors[Random.Range(0, normalFloors.Count)] : randomizedFloors[Random.Range(0, easyRandomFloors.Count)];
                break;
            case 2:
                platform = hardFloors[Random.Range(0, hardFloors.Count)];
                break;
            case 3:
                platform = deathFloors[Random.Range(0, deathFloors.Count)];
                break;
            }

        Instantiate(platform, new Vector3(xPos, 0, 0), Quaternion.identity);
        platform.transform.Find("Base").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        connectNextFloor = platform.GetComponent<MovingPlatform>().isConnected;
        if (connectNextFloor) nextFloor = platform.GetComponent<MovingPlatform>().connectedPlatform;
    }

    private void SpawnRoofs(float xPos, int difficulty)
    {
        GameObject platform = null;

        if (connectNextRoof) platform = nextRoof;
        else switch (difficulty)
        {
            case 0:
                platform = easyRoofs[Random.Range(0, easyRoofs.Count)];
                break;
            case 1:
                platform = normalRoofs[Random.Range(0, normalRoofs.Count)];
                break;
            case 2:
                platform = hardRoofs[Random.Range(0, hardRoofs.Count)];
                break;
            case 3:
                platform = deathRoofs[Random.Range(0, deathRoofs.Count)];
                break;
            }

        Instantiate(platform, new Vector3(xPos, 0, 0), Quaternion.identity);
        platform.transform.Find("Base").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        connectNextRoof = platform.GetComponent<MovingPlatform>().isConnected;
        if (connectNextRoof) nextRoof = platform.GetComponent<MovingPlatform>().connectedPlatform;
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(collisionInfo.collider.name == "Base")
        {
            Destroy(collisionInfo.collider.transform.root.gameObject);
            if (collisionInfo.collider.gameObject.GetComponent<Transform>().root.gameObject.name.Contains("Ground")) SpawnFloors(79.99f - (float)(startPoint * 10), gameDifficultyLevel);
            else if (gameDifficultyLevel != -1 && collisionInfo.collider.gameObject.GetComponent<Transform>().root.gameObject.name.Contains("Roof")) SpawnRoofs(79.99f - (float)(startPoint * 10), gameDifficultyLevel);
        }
    }
}
