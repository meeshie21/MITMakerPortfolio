using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pressed;

    void Update()
    {
        Debug.Log(pressed);
    }

    public void OnPointerDown(PointerEventData data)
    {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        pressed = false;
    }

    public bool IsPressed() { return this.pressed; }
}
