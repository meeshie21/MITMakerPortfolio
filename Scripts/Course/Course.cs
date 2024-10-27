using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Course : MonoBehaviour
{
    public int par;
    public Vector3 ballStartPos;
    public Vector3 cameraFullscreenPos;
    public Vector3 holePos;
    public string name;
    public float cameraFullscreenSize;
    public bool changeSizeOnFullscreen;
    public bool flyOff;
    public float minimapX;
    public float minimapY;
    public float minimapOrthoSize;


    public static Dictionary<int, string> scoreNames = new Dictionary<int, string>()
    {
        {0, "Par"},
        {-1, "Birdie"},
        {-2, "Eagle"},
        {-3, "Albatross"},
        {-4, "Condor"},
        {-5, "Ostrich"},
        {-6, "Phoenix"},

        {1, "Bogey"},
        {2, "Double Bogey"},
        {3, "Triple Bogey"},
        {4, "Quadruple Bogey"},
        {5, "Quintuple Bogey"},
        {6, "Sextuple Bogey"},
        {7, "Septuple Bogey"},
        {8, "Octuple Bogey"},
        {9, "Nonuple Bogey"},
        {10, "Decuple Bogey"},
        {11, "Undecuple Bogey"},
        {12, "Duodecuple Bogey"},
        {13, "Tridecuple Bogey"},
    };
}
