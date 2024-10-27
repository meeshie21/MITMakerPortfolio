using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class ImageMovement : MonoBehaviour
{
    public RectTransform[] images;
    public Transform player;

    public Camera camera;

    public bool iPhone6 = false;
    public bool iPhone6Plus = false;
    public bool iPhone7 = false;
    public bool iPhone7Plus = false;
    public bool iPhone8 = false;
    public bool iPhone8Plus = false;
    public bool iPhoneX = false;
    public bool iPhone11 = false;
    public bool iPhone12 = false;


    void Start()
    {
        
    }

    void Update()
    {
        if(Screen.width == 828 && Screen.height == 1792)
        {
            camera.orthographic = true;
            camera.orthographicSize = 18f;

            foreach (RectTransform rt in images)
            {
            rt.position = new Vector3(player.position.x * 55, player.position.z + 575f, rt.position.z);
            }
        }

        if (Screen.width == 1125 && Screen.height == 2436)
        {
            camera.orthographic = true;
            camera.orthographicSize = 18f;

            foreach (RectTransform rt in images)
            {
                rt.position = new Vector3(player.position.x * 75, player.position.z + 775f, rt.position.z);
            }
        }

        if (Screen.width == 750 && Screen.height == 1334)
        {
            camera.orthographic = true;
            camera.orthographicSize = 15f;

            foreach (RectTransform rt in images)
            {
                rt.position = new Vector3(player.position.x * 49, player.position.z + 375f, rt.position.z);
            }
        }

        if (Screen.width == 640 && Screen.height == 1136)
        {
            camera.orthographic = true;
            camera.orthographicSize = 15f;

            foreach (RectTransform rt in images)
            {
                rt.position = new Vector3(player.position.x * 42, player.position.z + 325f, rt.position.z);
            }
        }

        if (Screen.width == 1080 && Screen.height == 1920)
        {
            camera.orthographic = true;
            camera.orthographicSize = 15f;

            foreach (RectTransform rt in images)
            {
                rt.position = new Vector3(player.position.x * 72, player.position.z + 530f, rt.position.z);
            }
        }

        if (Screen.width == 1242 && Screen.height == 2688)
        {
            camera.orthographic = true;
            camera.orthographicSize = 18f;

            foreach (RectTransform rt in images)
            {
                rt.position = new Vector3(player.position.x * 82, player.position.z + 850f, rt.position.z);
            }
        }

        if (Screen.width == 1080 && Screen.height == 2340)
        {
            camera.orthographic = true;
            camera.orthographicSize = 18f;

            foreach (RectTransform rt in images)
            {
                rt.position = new Vector3(player.position.x * 72, player.position.z + 725f, rt.position.z);
            }
        }

        if (Screen.width == 1284 && Screen.height == 2778)
        {
            camera.orthographic = true;
            camera.orthographicSize = 18f;

            foreach (RectTransform rt in images)
            {
                rt.position = new Vector3(player.position.x * 85, player.position.z + 850f, rt.position.z);
            }
        }

        if (Screen.width == 1170 && Screen.height == 2532)
        {
            camera.orthographic = true;
            camera.orthographicSize = 18f;

            foreach (RectTransform rt in images)
            {
                rt.position = new Vector3(player.position.x * 78, player.position.z + 800f, rt.position.z);
            }
        }
    }
}
