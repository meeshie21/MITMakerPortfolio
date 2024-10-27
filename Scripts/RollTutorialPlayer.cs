using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RollTutorialPlayer : MonoBehaviour
{
    private Animator animator;

    private float startX, startY;
    private Vector2 startTouchPos, endTouchPos;
    [SerializeField] private float tolerance;
    [SerializeField] private float thrust, thrustReduction;

    private bool jumping, rolling, rollWhenHitGround;
    private int jumpCount;

    [SerializeField] private float jumpTolerance;

    [SerializeField] private Splat splat;

    private float offscreenTolerance = -4f;

    private bool visibleGameOver;

    public static bool started;
    public static bool gameOver;
    private float startSpeed;

    [SerializeField] private TMP_Text instructions;
    [SerializeField] private Fade fade;

    // Start is called before the first frame update
    void Start()
    {
        Vibration.Init();
        started = false;
        gameOver = false;
        startSpeed = 5f;
        ObstacleGroup.rotSpeed = 150f;
       
        startX = transform.position.x;
        startY = transform.position.y;

        animator = GetComponent<Animator>();
        animator.SetBool("Running", true);
        animator.SetBool("Jumping", false);
        animator.SetBool("Rolling", false);
        animator.SetBool("Flying", false);

        jumping = false;
        rolling = false;
        rollWhenHitGround = false;
        visibleGameOver = false;

        jumpCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        instructions.text = "Swipe down to roll under obstacles.";

        if(GetComponent<SpriteRenderer>().isVisible == false && transform.position.x < offscreenTolerance && !visibleGameOver)
        {
            gameOver = true;
            visibleGameOver = true;
            Vibration.Vibrate();

            GetComponent<Animator>().speed = 0f;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            MovingPlatform.speed = 0f;
            ObstacleGroup.rotSpeed = 0f;
            StartCoroutine(ReloadScene());
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Min(transform.position.x, 0f), transform.position.y, transform.position.z), 1);
        transform.rotation = Quaternion.identity;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    endTouchPos = touch.position;
                    if (GetComponent<Rigidbody2D>().gravityScale == 1 && startTouchPos.y - endTouchPos.y > tolerance && animator.GetBool("Jumping") == false)
                    {
                        if (SceneManager.GetActiveScene().buildIndex == 0) break;
                        animator.SetBool("Running", false);
                        animator.SetBool("Jumping", false);
                        animator.SetBool("Rolling", true);

                        Vibration.VibratePeek();
                        Debug.Log("Swipe Down"); //SWIPE DOWN WHEN ON GROUND
                    }

                    break;
                default: break;
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            animator.SetBool("Running", true);
            animator.SetBool("Jumping", false);
            animator.SetBool("Rolling", false);
        }
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(collisionInfo.collider.tag == "Obstacle")
        {
            gameOver = true;
            Vibration.Vibrate();

            transform.DetachChildren();
            splat.BloodSplat();
            splat.gameObject.SetActive(true);
            GetComponent<Animator>().speed = 0f;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            MovingPlatform.speed = 0f;
            ObstacleGroup.rotSpeed = 0f;
            StartCoroutine(ReloadScene());
        }

        else if (collisionInfo.collider.tag == "Box")
        {
            Destroy(collisionInfo.collider.gameObject);
            PlayerPrefs.SetString("tutorial", "ready");
            fade.LoadScene(7);
        }
    }
    
    void OnCollisionStay2D(Collision2D collisionInfo)
    {
        jumping = false;
        jumpCount = 0;

        if (collisionInfo.collider.tag == "Floor" && !animator.GetCurrentAnimatorStateInfo(0).IsName("Roll")) //check if roll animation is playing
        {
            animator.SetBool("Running", true);
            animator.SetBool("Jumping", false);
            animator.SetBool("Rolling", false);
        }
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(0.75f);
        fade.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
