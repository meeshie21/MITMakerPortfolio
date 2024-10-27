using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    private bool fullscreen;
    private Ball ball;
    private Vector3 fullCoursePos;

    private float targetSize;
    private float sizeLerpRate;

    void Start()
    {
        GetComponent<RectTransform>().position = new Vector3(GameObject.FindObjectOfType<Course>().GetComponent<Course>().minimapX, GameObject.FindObjectOfType<Course>().GetComponent<Course>().minimapY, 0f);

        ball = GameObject.FindObjectOfType<Ball>().GetComponent<Ball>();
        fullscreen = true;
        fullCoursePos = GameObject.FindObjectOfType<Course>().GetComponent<Course>().cameraFullscreenPos;
        transform.position = fullCoursePos;

        SetTargetSize(GameObject.FindObjectOfType<Course>().GetComponent<Course>().minimapOrthoSize, Ball.CAM_LERP_RATE);

    }

    void Update()
    {
        transform.parent.rotation = Quaternion.Euler(0, 0, 0);
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, targetSize, sizeLerpRate);
        transform.position = fullCoursePos;
    }

    public void SetTargetSize(float size, float lerpRate)
    {
        targetSize = size;
        sizeLerpRate = lerpRate;
    }
}
