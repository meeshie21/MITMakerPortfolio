using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject startPanel;

    [SerializeField] private TMP_Text par;
    [SerializeField] private TMP_Text startPar;
    [SerializeField] private TMP_Text courseName;
    [SerializeField] private TMP_Text shotsTaken;
    [SerializeField] private TMP_Text scoreName;
    [SerializeField] private TMP_Text endNumber;

    [SerializeField] private GameObject takeShot;
    [SerializeField] private GameObject resetRound;

    [SerializeField] private GameObject forceBar;
    [SerializeField] private Image forceBarFill;

    private int numVibrations;


    private Ball ball;
    private CameraManager cam;

    private float fillLerpRate = 0.15f;

    // Start is called before the first frame update
    public void Initialize()
    {
        ball = GameObject.FindObjectOfType<Ball>();
        cam = GameObject.FindObjectOfType<CameraManager>();
        par.text = "PAR " + GameObject.FindObjectOfType<Course>().GetComponent<Course>().par.ToString();
        startPar.text = "PAR " + GameObject.FindObjectOfType<Course>().GetComponent<Course>().par.ToString();
        courseName.text = GameObject.FindObjectOfType<Course>().GetComponent<Course>().name;

        numVibrations = 0;
        Vibration.Init();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        startPanel.gameObject.SetActive(!cam.HasStarted());
        shotsTaken.text = "SHOTS TAKEN: " + ball.GetShotsTaken().ToString();
        forceBarFill.fillAmount = Mathf.Lerp(forceBarFill.fillAmount, (ball.GetShotMagnitude()/Ball.MAX_SHOT_MAGNITUDE), fillLerpRate);

        if (forceBarFill.fillAmount == 0f) numVibrations = 0;

        if(forceBarFill.fillAmount <= 0.5f)
        {
            forceBarFill.color = Color.Lerp(Color.green, Color.yellow, forceBarFill.fillAmount * 2);
        }
        else
        {
            forceBarFill.color = Color.Lerp(Color.yellow, Color.red, 2 * (forceBarFill.fillAmount - 0.5f));
        }
#if UNITY_IPHONE
        if(forceBar.activeSelf && forceBarFill.fillAmount != 0f)
        {
            Vibration.Init();
            if(forceBarFill.fillAmount <= 0.6f) Vibration.VibrateIOS(ImpactFeedbackStyle.Light);
            else if(forceBarFill.fillAmount <= 0.85f) Vibration.VibrateIOS(ImpactFeedbackStyle.Medium);
            else Vibration.VibrateIOS(ImpactFeedbackStyle.Heavy);

            numVibrations++;
        }

        else 
        {}
        
#endif

        if (ball.IsLaunchable() && !ball.MadeValidTouch() && !ball.AlreadyLaunched()) takeShot.GetComponent<TMP_Text>().text = ball.GetShotsTaken() == 0 ? "Pull back for power, let go to launch." : "Take your next shot.";
        else takeShot.GetComponent<TMP_Text>().text = "";

        if (cam.GetFullscreen() && cam.DoneLerping()) resetRound.GetComponent<TMP_Text>().text = cam.HasStarted() ? "Tap anywhere to continue round." : "Tap anywhere to start round.";
        else resetRound.GetComponent<TMP_Text>().text = "";

        forceBar.SetActive(ball.IsLaunchable() && ball.MadeValidTouch() && !ball.AlreadyLaunched());
        if (!forceBar.activeSelf) forceBarFill.fillAmount = 0f; 
    }

    public void EndGameUI()
    {
        endPanel.gameObject.SetActive(true);
        int scoreDifference = ball.GetShotsTaken() - GameObject.FindObjectOfType<Course>().GetComponent<Course>().par;
        if (ball.GetShotsTaken() == 1) scoreName.text = "Hole-in-One!";
        else if (scoreDifference != null && scoreDifference <= 13 && scoreDifference >= -6) scoreName.text = Course.scoreNames[scoreDifference];
        else scoreName.text = scoreDifference > 0 ? "+" + scoreDifference.ToString() : "-" + scoreDifference.ToString();

        endNumber.text = (scoreDifference >= 0) ? "+" + scoreDifference.ToString() : scoreDifference.ToString();
        takeShot.SetActive(false);
        resetRound.SetActive(false);
    }
}
