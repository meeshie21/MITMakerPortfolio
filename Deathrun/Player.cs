using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
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

    private float startPosY = -3.845f;
    [SerializeField] private float xDiffTolerance;

    private int floorsCount;

    private bool visibleGameOver;

    public static bool started;
    public static bool gameOver;
    private float startSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Vibration.Init();
        started = false;
        gameOver = false;
        startSpeed = MovingPlatform.speed;
        MovingPlatform.speed = 0f;
        GetComponent<Animator>().speed = 0f;
        floorsCount = 0;

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
        if (!started && Input.touchCount > 0)
        {
            Vibration.VibratePeek();
            MovingPlatform.speed = startSpeed;
            GetComponent<Animator>().speed = 1f;
            started = true;
        }

        if(GetComponent<SpriteRenderer>().isVisible == false && transform.position.x < offscreenTolerance && !visibleGameOver)
        {
            gameOver = true;
            visibleGameOver = true;
            Vibration.Vibrate();

            GetComponent<Animator>().speed = 0f;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            GameObject.FindObjectOfType<GameManager>().GameOver();
        }

        //VISIBLE GAME OVER IS USED AS A TRIGGER IN THE CASES WHERE WE COLLIDE WITH THE OBSTACLE FLOOR GAMEOBJECTS
        //TO ENSURE THAT THIS CODE ONLY RUNS ONCE
        //THIS PREVENTS US FROM CREATING ANOTHER BOOLEAN
        if(floorsCount != 0 && transform.position.x < xDiffTolerance && !visibleGameOver)//FLOORS COUNT DOES NOT INCLUDE BASE
        {
            gameOver = true;
            visibleGameOver = true;
            Vibration.Vibrate();

            transform.DetachChildren();
            splat.BloodSplat();
            splat.gameObject.SetActive(true);
            GetComponent<Animator>().speed = 0f;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            GameObject.FindObjectOfType<GameManager>().GameOver();
        }

        else if(floorsCount == 0) transform.position = Vector3.Lerp(transform.position, new Vector3(0f, transform.position.y, transform.position.z), 1);
        else transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Min(0f, transform.position.x), transform.position.y, transform.position.z), 1);

        transform.rotation = Quaternion.identity;

        /*if (GetComponent<Rigidbody2D>().gravityScale == 1)
        {
            animator.SetBool("Flying", false);
        }

        else
        {
            animator.SetBool("Flying", true);
            animator.SetBool("Running", false);
            animator.SetBool("Jumping", false);
            animator.SetBool("Rolling", false);
        }*/

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    if (touch.position.y > startTouchPos.y) //SWIPE UP
                    {
                        //DO SOMETHING IF NOT RESPONSIVE IN THESE AREAS
                    }
                    else if (touch.position.y < startTouchPos.y) //SWIPE DOWN
                    {
                        
                    }
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
                        rollWhenHitGround = false;
                        jumping = true;
                        jumpCount++;
                        GetComponent<Rigidbody2D>().AddForce(transform.up * thrust);
                        Vibration.VibratePop();
                        Debug.Log("Swipe Up"); //SWIPE UP
                    }

                    else if (GetComponent<Rigidbody2D>().gravityScale == 1 && endTouchPos.y - startTouchPos.y > tolerance && jumping)// && !animator.GetCurrentAnimatorStateInfo(0).IsName("Roll")) //check if roll animation is playing
                    {
                        if (jumpCount >= 1) return;

                        rollWhenHitGround = false;
                        jumping = true;
                        jumpCount++;
                        GetComponent<Rigidbody2D>().AddForce(transform.up * (thrust - thrustReduction));
                        Vibration.VibratePop();
                        Debug.Log("Swipe Up"); //SWIPE UP IN AIR
                    }

                    else if (GetComponent<Rigidbody2D>().gravityScale == 1 && startTouchPos.y - endTouchPos.y > tolerance && animator.GetBool("Jumping") == false)
                    {
                        if (SceneManager.GetActiveScene().buildIndex == 0) break;
                        animator.SetBool("Running", false);
                        animator.SetBool("Jumping", false);
                        animator.SetBool("Rolling", true);

                        Vibration.VibratePeek();
                        Debug.Log("Swipe Down"); //SWIPE DOWN WHEN ON GROUND
                    }

                    else if (GetComponent<Rigidbody2D>().gravityScale == 1 && startTouchPos.y > endTouchPos.y)
                    {
                        if (SceneManager.GetActiveScene().buildIndex == 0) break;
                        GetComponent<Rigidbody2D>().AddForce(transform.up * -1 * thrust);
                        rollWhenHitGround = true;
                        Debug.Log("Swipe Down"); //SWIPE DOWN WHEN IN AIR
                    }

                    break;
                default: break;
            }
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            animator.SetBool("Running", true);
            animator.SetBool("Jumping", false);
            animator.SetBool("Rolling", false);

            rollWhenHitGround = false;
        }

        else rolling = false;
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
            GameObject.FindObjectOfType<GameManager>().GameOver();
        }

        if (collisionInfo.collider.tag == "Floor" /*&& !collisionInfo.collider.gameObject.name.Contains("Base")
            && !collisionInfo.collider.gameObject.name.Equals("Floor") && !collisionInfo.collider.gameObject.name.Equals("Roof")*/
            && collisionInfo.collider.gameObject.name.IndexOf("box", StringComparison.OrdinalIgnoreCase) >= 0
            && collisionInfo.collider.gameObject.GetComponent<DontRemove>() == null) floorsCount++;

        if (collisionInfo.collider.tag == "Floor" && rollWhenHitGround)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Jumping", false);
            animator.SetBool("Rolling", true);

            Vibration.VibratePeek();
        }
    }
    
    void OnCollisionStay2D(Collision2D collisionInfo)
    {
        jumping = false;
        jumpCount = 0;

        if (collisionInfo.collider.tag == "Floor" && !animator.GetCurrentAnimatorStateInfo(0).IsName("Roll") && !rollWhenHitGround) //check if roll animation is playing
        {
            animator.SetBool("Running", true);
            animator.SetBool("Jumping", false);
            animator.SetBool("Rolling", false);
        }
    }

    void OnCollisionExit2D(Collision2D collisionInfo)
    {
        if (collisionInfo.collider.tag == "Floor" /*&& !collisionInfo.collider.gameObject.name.Contains("Base") 
            && !collisionInfo.collider.gameObject.name.Equals("Floor") && !collisionInfo.collider.gameObject.name.Equals("Roof")*/
            && collisionInfo.collider.gameObject.name.IndexOf("box", StringComparison.OrdinalIgnoreCase) >= 0
            && collisionInfo.collider.gameObject.GetComponent<DontRemove>() == null) floorsCount--;

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
}
