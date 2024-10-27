using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splat : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = null;
        gameObject.SetActive(false);
    }

    public void BloodSplat()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
