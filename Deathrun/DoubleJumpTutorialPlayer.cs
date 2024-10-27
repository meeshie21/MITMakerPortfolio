using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DoubleJumpTutorialPlayer : MonoBehaviour
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
        instructions.text = "Swipe up twice to double jump.";

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
                    if (Mathf.Abs(endTouchPos.y - startTouchPos.y) < tolerance)
                    {
                        //GetComponent<Rigidbody2D>().gravityScale *= -1;
                        Debug.Log("Tap");
                    }

                    else if (GetComponent<Rigidbody2D>().gravityScale == 1 && endTouchPos.y - startTouchPos.y > tolerance && !jumping)// && !animator.GetCurrentAnimatorStateInfo(0).IsName("Roll")) //check if roll animation is playing
                    {
                        jumping = true;
                        jumpCount++;
                        GetComponent<Rigidbody2D>().AddForce(transform.up * thrust);
                        Vibration.VibratePop();
                        Debug.Log("Swipe Up"); //SWIPE UP
                    }

                    else if (GetComponent<Rigidbody2D>().gravityScale == 1 && endTouchPos.y - startTouchPos.y > tolerance && jumping)// && !animator.GetCurrentAnimatorStateInfo(0).IsName("Roll")) //check if roll animation is playing
                    {
                        if (jumpCount >= 1) return;

                        jumping = true;
                        jumpCount++;
                        GetComponent<Rigidbody2D>().AddForce(transform.up * (thrust - thrustReduction));
                        Vibration.VibratePop();
                        Debug.Log("Swipe Up"); //SWIPE UP IN AIR
                    }

                    break;
                default: break;
            }
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
            PlayerPrefs.SetString("tutorial", "roll");
            fade.LoadScene(6);
        }
    }
    
    void OnCollisionStay2D(Collision2D collisionInfo)
    {
        jumping = false;
        jumpCount = 0;

        animator.SetBool("Running", true);
        animator.SetBool("Jumping", false);
        animator.SetBool("Rolling", false);
    }

    void OnCollisionExit2D(Collision2D collisionInfo)
    {
        if (collisionInfo.collider.tag == "Floor" && GetComponent<Rigidbody2D>().gravityScale != -1)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Jumping", true);
            animator.SetBool("Rolling", false);

            StartCoroutine(CheckForJump());
        }
    }

    IEnumerator CheckForJump()
    {
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Checking Jump Height");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
        float distance = Mathf.Abs(hit.point.y - transform.position.y);

        jumping = (distance > jumpTolerance);
        if (jumping) jumpCount++;
        StopCoroutine(CheckForJump());
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(0.75f);
        fade.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
