using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private bool fullscreen;
    private Vector3 originalPos;
    private Ball ball;
    private Vector3 fullCoursePos;

    private float doneLerpingErrorMargin = 0.05f;
    private bool doneLerping;

    private float targetSize;
    private float sizeLerpRate;

    private bool hasStarted;

    public void SetFullscreen(bool fullscreen) { this.fullscreen = fullscreen; }
    public bool GetFullscreen() { return this.fullscreen; }
    public bool HasStarted() { return this.hasStarted; }

    void Start()
    {
        ball = GameObject.FindObjectOfType<Ball>().GetComponent<Ball>();
        fullscreen = true;
        originalPos = transform.localPosition;
        fullCoursePos = GameObject.FindObjectOfType<Course>().GetComponent<Course>().cameraFullscreenPos;
        fullCoursePos = new Vector3(fullCoursePos.x, fullCoursePos.y + /*GameObject.FindObjectOfType<Course>().GetComponent<Course>().yOffset*/ 0.75f, fullCoursePos.z);
        transform.position = fullCoursePos;
        doneLerping = false;
        hasStarted = false;
    }

    void Update()
    {
        transform.parent.rotation = Quaternion.Euler(0, 0, 0);

        if (Input.touchCount > 0 && doneLerping)
        {
            Vibration.Init();
            Vibration.VibratePop();
            fullscreen = false;
            if (!hasStarted) hasStarted = true;
            //OPTIONAL GAME MANAGER START GAME HERE
        }

        else if (fullscreen && !ball.ButtonIsPressed()) GoToFullscreen();

        if (!fullscreen)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, sizeLerpRate);
            doneLerping = (transform.localPosition - originalPos).magnitude <= doneLerpingErrorMargin;
        }

        if (GameObject.FindObjectOfType<Course>().GetComponent<Course>().changeSizeOnFullscreen && fullscreen)
        {
            SetTargetSize(GameObject.FindObjectOfType<Course>().GetComponent<Course>().cameraFullscreenSize, Ball.CAM_LERP_RATE);
        }
        
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, targetSize, sizeLerpRate);
    }

    public void GoToFullscreen()
    {
        transform.position = Vector3.Lerp(transform.position, fullCoursePos, Ball.CAM_LERP_RATE);
        doneLerping = (transform.position - fullCoursePos).magnitude <= doneLerpingErrorMargin;
    }


    public void SetTargetSize(float size, float lerpRate)
    {
        targetSize = size;
        sizeLerpRate = lerpRate;
    }

    public bool DoneLerping() { return this.doneLerping; }

}
