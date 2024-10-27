using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public RectTransform rectTransform;
    public bool isRotating = false;

    void Start()
    {
        StartCoroutine(Rotate());
    }

    void Update()
    {
        if (isRotating) return;
        StartCoroutine(Rotate());
    }

    public IEnumerator Rotate()
    {
        isRotating = true;

        rectTransform.Rotate(0, 0, -2f);

        yield return new WaitForSeconds(0.01f);

        isRotating = false;
    }
}
