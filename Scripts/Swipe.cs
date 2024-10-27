using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    public Vector2 startPos;
    public Vector2 endPos;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public rbSupplier DetectSwipe(Touch touch)
    {
        //Touch touch = Input.GetTouch(0);

        if (touch.rawPosition.y - touch.position.y < 0)
        {
            int length = Mathf.Abs((int)(touch.rawPosition.y - touch.position.y));
            return new rbSupplier(length, (touch.rawPosition.x - touch.position.x));
        }

        else return new rbSupplier(0, 0f);

    }
}

public class rbSupplier
{
    public int length;
    public float turnFactor;

    public rbSupplier(int length, float turnFactor)
    {
        this.length = length;
        this.turnFactor = turnFactor;
    }
}
