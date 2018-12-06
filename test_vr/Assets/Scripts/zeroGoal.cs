using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class zeroGoal : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject drone;
    public float speed;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    //  PID
    void Update()
    {
        rb.velocity = speed*(drone.transform.position - transform.position);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);


        float e = drone.transform.localEulerAngles.y - transform.localEulerAngles.y;
        if (e > 180) e -= 360.0f;
        else if (e < -180) e += 360.0f;
        rb.angularVelocity = speed/180 * new Vector3(0, drone.transform.localEulerAngles.y - transform.localEulerAngles.y, 0);
    }
}