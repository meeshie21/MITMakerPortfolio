using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    void Start()
    {
        Course course = GameObject.FindObjectOfType<Course>().GetComponent<Course>();
        transform.position = new Vector3(course.holePos.x, transform.position.y, course.holePos.z);
    }
}
