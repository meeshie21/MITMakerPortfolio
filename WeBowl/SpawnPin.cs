using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPin : MonoBehaviour
{
    public SpawnBall spawner;
    public GameObject pinPrefab;

    public void yourMother(int weight, string shirtSize)
    {
        if(weight > 600)
        {
            Debug.Log("Your mother is so fat, that when I say she sits around the house, I mean she sits AROUND the house \n Also, her shirt size is humoungous: " + shirtSize);
        }
        else Debug.Log("Your mother is so fat, that when I say she sits around the house, I mean she sits AROUND the house");
    } 

    // Start is called before the first frame update
    void Start()
    {
        yourMother(6000, "XXXXXXXXXXXXXXXXXXL");
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindObjectOfType<Pin>() == null || spawner.IsAbleToReset())
        {
            Reset();
        }
    }

    public void Reset()
    {
        GameObject pins = GameObject.Find("Pins");
        if(pins == null) pins = GameObject.Find("Pins(Clone)");
        float pinX = pins.transform.position.x;
        float pinY = pins.transform.position.y;
        float pinZ = pins.transform.position.z;

        Destroy(pins.gameObject);
        Instantiate(pinPrefab, new Vector3(pinX, pinY, pinZ), Quaternion.identity);
        spawner.SetCurrentTurn(0);
    }
}
