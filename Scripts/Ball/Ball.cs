using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    private Camera cam;
    private GameObject floor;
    private Rigidbody rb;
    private Vector3 firstTouchPosition;
    private Vector3 lastTouchPosition;
    private bool launchable;
    private bool launched;
    [SerializeField] private float ERROR_MARGIN;
    [SerializeField] private float ANGULAR_ERROR_MARGIN;
    private int shotsTaken;
    private float shotAngle;
    private float shotMagnitude;
    private float offsetRatio;
    private const float MAG_OFFSET = 0f;
    private bool reachedHole;
    private bool playedHoleSound;
    private float gamePosY;
    private float maxCamSize = 2f;
    private float minCamSize = 1.5f;
    private float endCamSize = 6f;
    private float smoothAngleRate = 0.05f;
    private float smoothAngleVelocity = 0.5f;
    private float shotMagShiftRatio = 80f;
    private bool buttonPressedOnTouch;
    private bool alreadyLaunched;
    private Touch tempTouch;
    private bool validTouch;
    private int barriersCount;
    private int floorsCount;
    [SerializeField] private float forceMultiplier;
    [SerializeField] private float lengthDecreaseFactor;
    private Vector3 lastPos;

    public static float CAM_LERP_RATE = 0.075f;
    public static float MAX_SHOT_MAGNITUDE = 350f;

    public int GetShotsTaken() { return this.shotsTaken; }
    public float GetShotAngle() { return this.shotAngle; }
    public float GetShotMagnitude() { return this.shotMagnitude; }
    public bool IsLaunchable() { return this.launchable; }
    public bool AlreadyLaunched() { return this.alreadyLaunched; }
    public bool ReachedHole() { return this.reachedHole; }
    public bool MadeValidTouch() { return this.validTouch; }

    // Start is called before the first frame update
    void Start()
    {
        Vibration.Init();
        transform.position = GameObject.FindObjectOfType<Course>().GetComponent<Course>().ballStartPos;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
        lastTouchPosition = cam.WorldToScreenPoint(transform.position);
        gamePosY = transform.position.y;
        reachedHole = false;
        playedHoleSound = false;
        launched = false;
        alreadyLaunched = false;
        buttonPressedOnTouch = false;
        validTouch = false;
        floor = GameObject.Find("Floor");
        barriersCount = 0;
        floorsCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Walls: " + barriersCount + "|| Floors: " + floorsCount);
        launchable = (rb.velocity.z <= ERROR_MARGIN && rb.velocity.x <= ERROR_MARGIN && rb.angularVelocity.magnitude <= ANGULAR_ERROR_MARGIN) && cam.GetComponent<CameraManager>().DoneLerping() && !cam.GetComponent<CameraManager>().GetFullscreen();
        if (launchable) alreadyLaunched = false;

        if (reachedHole)
        {
            floor.SetActive(false);
            cam.GetComponent<CameraManager>().SetFullscreen(true);
            if (cam.orthographicSize <= endCamSize - 0.1f) cam.GetComponent<CameraManager>().SetTargetSize(endCamSize, CAM_LERP_RATE/10f);
            Vector3 holePos = GameObject.Find("EnterHoleTrigger").transform.position;
            Vector3 target = new Vector3(holePos.x, holePos.y, holePos.z);
            transform.position = Vector3.Lerp(transform.position, target, 1f);

            if(transform.position.y < gamePosY && ! playedHoleSound)
            {
                StartCoroutine(VibrateNope());
                playedHoleSound = true;
            }

            return;
        }

        if(!cam.GetComponent<CameraManager>().GetFullscreen())
        {
            if ((rb.velocity.z <= ERROR_MARGIN && rb.velocity.x <= ERROR_MARGIN && rb.angularVelocity.magnitude <= ANGULAR_ERROR_MARGIN) || !cam.GetComponent<CameraManager>().DoneLerping()) cam.GetComponent<CameraManager>().SetTargetSize(maxCamSize, CAM_LERP_RATE);
            else cam.GetComponent<CameraManager>().SetTargetSize(minCamSize, CAM_LERP_RATE * 2f);
        }

        floor.SetActive(true);

        if (Input.touchCount > 0 && !launchable)
        {
            tempTouch = Input.GetTouch(0);
            validTouch = false;
        }

        if (!launchable) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            buttonPressedOnTouch =  ButtonIsPressed();
            if (buttonPressedOnTouch || touch.rawPosition == tempTouch.rawPosition) return;
            validTouch = true;

            if (touch.phase == TouchPhase.Began) firstTouchPosition = touch.position;
            lastTouchPosition = touch.position;
            CalcShotAngle(firstTouchPosition, lastTouchPosition);

            if (touch.phase == TouchPhase.Ended && !launched)
            {
                if (lastTouchPosition == firstTouchPosition) return;
                LaunchBall(firstTouchPosition, lastTouchPosition);
            }

            else if (touch.phase != TouchPhase.Ended) launched = false;
        }

        if (Input.touchCount <= 0 && buttonPressedOnTouch) buttonPressedOnTouch = false;
    }

    private void LaunchBall(Vector3 startPos, Vector3 endPos)
    {
        if (!launchable) return;

        validTouch = false;
        alreadyLaunched = true;

        lastPos = transform.position;

        Vector3 ballPos = cam.WorldToScreenPoint(transform.position);
        float xForce = (endPos.y - startPos.y) / lengthDecreaseFactor;
        float yForce = (startPos.x - endPos.x) / lengthDecreaseFactor;

        xForce *= Mathf.Max(cam.orthographicSize, minCamSize) / minCamSize;
        yForce *= Mathf.Max(cam.orthographicSize, minCamSize) / minCamSize;

        Vibration.VibratePeek();
        rb.AddForce(Quaternion.AngleAxis(-1f * cam.transform.eulerAngles.y, Vector3.up) * new Vector3(xForce * forceMultiplier * offsetRatio, 0, yForce * forceMultiplier * offsetRatio));

        gamePosY = transform.position.y;
        launched = true;
        shotsTaken++;
    }

    private void CalcShotAngle(Vector3 startPos, Vector3 endPos)
    {
        Vector3 ballPos = cam.WorldToScreenPoint(transform.position);
        float xForce = (endPos.y - startPos.y) / lengthDecreaseFactor;
        float yForce = (startPos.x - endPos.x) / lengthDecreaseFactor;

        shotAngle = Mathf.SmoothDampAngle(shotAngle, -1f * cam.transform.eulerAngles.y + Mathf.Atan2(xForce, yForce) * Mathf.Rad2Deg, ref smoothAngleVelocity, smoothAngleRate);

        xForce *= Mathf.Max(cam.orthographicSize, minCamSize) / minCamSize;
        yForce *= Mathf.Max(cam.orthographicSize, minCamSize) / minCamSize;

        float originalShotMagnitude = Mathf.Sqrt(Mathf.Pow(xForce, 2) + Mathf.Pow(yForce, 2));
        shotMagnitude = Mathf.Sqrt(Mathf.Pow(xForce, 2) + Mathf.Pow(yForce, 2)) - MAG_OFFSET;
        shotMagnitude = Mathf.Min(shotMagnitude, MAX_SHOT_MAGNITUDE);
        offsetRatio = shotMagnitude / originalShotMagnitude;

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, Mathf.Max(minCamSize, shotMagnitude / shotMagShiftRatio), CAM_LERP_RATE);
    }

    public bool ButtonIsPressed()
    {
        if(Input.touchCount > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
        }

        foreach(UIButton b in GameObject.FindObjectsOfType(typeof(UIButton)) as UIButton[])
        {
            if (b.IsPressed()) return true;
        }

        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnterHole")
        {
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            reachedHole = true;
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (reachedHole) return;
        if (collisionInfo.collider.tag == "Barrier")
        {
            barriersCount++;
            if (barriersCount == 1) Vibration.VibratePop();
        }

        else if (collisionInfo.collider.tag == "Wall")
        {
            floorsCount++;
            if (shotsTaken == 0) return;
            if (floorsCount == 1) Vibration.VibratePop();
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (reachedHole) return;
        if (collisionInfo.collider.tag == "Wall") floorsCount--;
        if (collisionInfo.collider.tag == "Barrier") barriersCount--;
    }

    IEnumerator VibrateNope()
    {
        Vibration.VibrateNope();
        Vibration.VibratePeek();
        yield return new WaitForSeconds(0.1f);
        Vibration.VibrateNope();
        StopCoroutine(VibrateNope());
    }
}
