using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Touch touch;

    void Start()
    {
        
    }

    void Update()
    {
        if(transform.position.x >= 14.5)
        {
            transform.position = new Vector3(14.5f, transform.position.y, transform.position.z);
        }

        if (transform.position.x <= 0)
        {
            transform.position = new Vector3(0f, transform.position.y, transform.position.z);
        }

        if (Input.GetKey("d"))
        {
            transform.position =
                                Vector3.Lerp(transform.position, new Vector3(
                                    transform.position.x + 3.25f,
                                    transform.position.y,
                                    transform.position.z), 8f * Time.deltaTime);
        }

        if (Input.GetKey("a"))
        {
            transform.position =
                                Vector3.Lerp(transform.position, new Vector3(
                                    transform.position.x - 3.25f,
                                    transform.position.y,
                                    transform.position.z), 8f * Time.deltaTime);
        }

        if (Screen.width == 828 && Screen.height == 1792)
        {

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position =
                        Vector3.Lerp(transform.position, new Vector3(
                            touch.position.x / 50,
                            transform.position.y,
                            transform.position.z), 8f * Time.deltaTime);
                }
            }
        }

        if (Screen.width == 1125 && Screen.height == 2436)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position =
                        Vector3.Lerp(transform.position, new Vector3(
                            touch.position.x / 65,
                            transform.position.y,
                            transform.position.z), 8f * Time.deltaTime);
                }
            }
        }

        if (Screen.width == 750 && Screen.height == 1334)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position =
                        Vector3.Lerp(transform.position, new Vector3(
                            touch.position.x / 60,
                            transform.position.y,
                            transform.position.z), 8f * Time.deltaTime);
                }
            }
        }

        if (Screen.width == 640 && Screen.height == 1136)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position =
                        Vector3.Lerp(transform.position, new Vector3(
                            touch.position.x / 55,
                            transform.position.y,
                            transform.position.z), 8f * Time.deltaTime);
                }
            }
        }

        if (Screen.width == 1080 && Screen.height == 1920)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position =
                        Vector3.Lerp(transform.position, new Vector3(
                            touch.position.x / 60,
                            transform.position.y,
                            transform.position.z), 8f * Time.deltaTime);
                }
            }
        }

        if (Screen.width == 1242 && Screen.height == 2688)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position =
                        Vector3.Lerp(transform.position, new Vector3(
                            touch.position.x / 65,
                            transform.position.y,
                            transform.position.z), 8f * Time.deltaTime);
                }
            }
        }

        if (Screen.width == 1080 && Screen.height == 2340)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position =
                        Vector3.Lerp(transform.position, new Vector3(
                            touch.position.x / 65,
                            transform.position.y,
                            transform.position.z), 8f * Time.deltaTime);
                }
            }
        }

        if (Screen.width == 1284 && Screen.height == 2778)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position =
                        Vector3.Lerp(transform.position, new Vector3(
                            touch.position.x / 65,
                            transform.position.y,
                            transform.position.z), 8f * Time.deltaTime);
                }
            }
        }

        if (Screen.width == 1170 && Screen.height == 2532)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position =
                        Vector3.Lerp(transform.position, new Vector3(
                            touch.position.x / 65,
                            transform.position.y,
                            transform.position.z), 8f * Time.deltaTime);
                }
            }
        }
    }
}
