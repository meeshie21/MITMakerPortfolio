using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] private float scaleFactor;
    [SerializeField] private float normalScale;
    [SerializeField] private float zConst;

    private float lineLerpRate = 0.1f;
    private float angleLerpRate = 1f;
    private float minLineScale;
    private float minLinePos;
    private Ball ball;
    private Transform line;
    private float colorLerpRate = 0.1f;
    private bool enableRenderers;

    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.FindObjectOfType<Ball>().GetComponent<Ball>();
        line = transform.Find("Line");
        minLineScale = line.localScale.z;
        minLinePos = line.localPosition.z;
        enableRenderers = false;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (MeshRenderer renderer in transform.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = enableRenderers;
            /*renderer.material.color = (ball.GetShotMagnitude() == Ball.MAX_SHOT_MAGNITUDE)
                ? Color.Lerp(renderer.material.color, new Color(1f, 0.4f, 0.4f, 1f), colorLerpRate)
                : Color.Lerp(renderer.material.color, Color.white, colorLerpRate);*/

            if (!renderer.enabled) line.localScale = new Vector3(line.localScale.x, line.localScale.y, minLineScale);
        }

        transform.eulerAngles = new Vector3(0f, Mathf.Lerp(transform.eulerAngles.y, ball.GetShotAngle(), angleLerpRate), 0f);

        line.localScale = new Vector3(line.localScale.x, line.localScale.y, Mathf.Lerp(line.localScale.z, Mathf.Min(zConst, normalScale / scaleFactor * ball.GetShotMagnitude()), lineLerpRate));
        line.localPosition = new Vector3(line.localPosition.x, line.localPosition.y, minLinePos * (line.localScale.z / minLineScale));

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.TransformDirection(-1 * Vector3.up));

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Did Hit");
            Debug.Log(hit.collider.gameObject.name);
        }

        else
        {
            Debug.Log("Did not Hit");
        }

        enableRenderers = Input.touchCount > 0 && ball.IsLaunchable() && !ball.ButtonIsPressed() && ball.MadeValidTouch();
    }
}
