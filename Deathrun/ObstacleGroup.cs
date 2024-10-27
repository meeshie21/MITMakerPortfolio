using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGroup : MonoBehaviour
{
    //private Color red = new Color(113, 17, 29, 255);
    private GameObject[] sprites;
    private PolygonCollider2D pc;
    private float sawDir;
    [SerializeField] private bool isSaw;
    [SerializeField] private bool singleObstacle;
    [SerializeField] private bool indestructible;
    [SerializeField] private bool rotated;

    public static float rotSpeed = 150f;

    //[SerializeField] private bool hangingObstacle;

    void Awake()
    {
        rotSpeed = 150f;

        if (!indestructible && Mathf.Sign(Random.Range(-2f, 2f)) == 1) Destroy(this.gameObject);

        if (singleObstacle) return;

        sprites = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++) sprites[i] = transform.GetChild(i).gameObject;
        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);

        GameObject spriteToAdd = sprites[Random.Range(0, sprites.Length - 1)];
        GetComponent<SpriteRenderer>().sprite = spriteToAdd.GetComponent<SpriteRenderer>().sprite;
        GetComponent<SpriteRenderer>().color = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        
        if(!isSaw)
        {
            float deltaY = spriteToAdd.transform.position.y;
            if (!rotated) transform.position = new Vector3(transform.position.x, deltaY, transform.position.z);
            //else transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + spriteToAdd.transform.localPosition.y, transform.localPosition.z);
        }

        if (isSaw)
        {
            float rand = Random.Range(-2f, 2f);
            sawDir = Mathf.Sign(rand);

            //GetComponent<AudioSource>().time = Random.Range(0f, GetComponent<AudioSource>().clip.length);
            //GetComponent<AudioSource>().Play();
        }

        pc = gameObject.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D;
    }

    void Update()
    {
        if (isSaw)
        {
            transform.Rotate(new Vector3(0f, 0f, rotSpeed * sawDir * Time.deltaTime), Space.Self);
        }
    }
}
