using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class position_pid : MonoBehaviour
{
    public GameObject goal;
    private UDPSend tcpclient;

    public float elapseTime = 0.016f;
    
    private float kp = 100.0f; // manual tuning: start with all 0, then try to set kp, if there is oscillation (overshoot + undershoot) minimize it with kd
    private float ki = 0.0f; // last try to set ki .. it would be small value though
    private float kd = 0.0f;

    private Vector3 integralPositionError;
    private Vector3 previousPositionError;
    private float integralOrientationError;
    private float previousOrientationError;

    // Use this for initialization
    void Start ()
    {
        integralPositionError = new Vector3(0, 0, 0);
        previousPositionError = new Vector3(0, 0, 0);
        integralOrientationError = 0.0f;
        previousOrientationError = 0.0f;

        tcpclient = gameObject.GetComponent<UDPSend>();
    }
    
    //  PID
    void Update()
    {
        //  compute position command using a simple PID
        Vector3 positionError = goal.transform.position - transform.position;
        Vector3 derivativeError = (positionError - previousPositionError) / elapseTime;
        Vector3 commandPosition = kp * positionError + ki * integralPositionError + kd * derivativeError;
        previousPositionError = positionError;
        integralPositionError += elapseTime * positionError;

        float orientationError = Mathf.Deg2Rad * (goal.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y) / 2.0f;
        float derivativeOrientationError = (orientationError - previousOrientationError) / elapseTime;
        float commandOrientation = kp * orientationError + ki * integralOrientationError + kd * derivativeOrientationError;
        previousOrientationError = orientationError;
        integralOrientationError += elapseTime * orientationError;

        // clamp command and integral error to prevent drone dammages
        float maxCommandMagnitude = 40.0f;
        float maxIntegralErrorMagnitude = 1.0f;

        commandPosition = clamp(commandPosition, new Vector3(-maxCommandMagnitude, -maxCommandMagnitude, -maxCommandMagnitude), new Vector3(maxCommandMagnitude, maxCommandMagnitude, maxCommandMagnitude));
        integralPositionError = clamp(integralPositionError, new Vector3(-maxIntegralErrorMagnitude, -maxIntegralErrorMagnitude, -maxIntegralErrorMagnitude), new Vector3(maxIntegralErrorMagnitude, maxIntegralErrorMagnitude, maxIntegralErrorMagnitude));

        commandOrientation = Mathf.Clamp(commandOrientation, -maxCommandMagnitude, maxCommandMagnitude);
        integralOrientationError = Mathf.Clamp(integralOrientationError, -maxIntegralErrorMagnitude, maxIntegralErrorMagnitude);

        //  compute local command
        Matrix4x4 localToWorld = Matrix4x4.Translate(transform.position) * Matrix4x4.Rotate(transform.rotation);
        Vector4 localPositionCommand = localToWorld.inverse * new Vector4(commandPosition.x, commandPosition.y, commandPosition.z, 0.0f);
        Vector3Int dummyInt = new Vector3Int(Mathf.RoundToInt(localPositionCommand.x), Mathf.RoundToInt(localPositionCommand.y), Mathf.RoundToInt(localPositionCommand.z));

        //  Send Command to NodeJS server
        if (tcpclient)
             tcpclient.SendMessage(dummyInt.x.ToString() + " " + dummyInt.y.ToString() + " " + dummyInt.z.ToString() + " " + Mathf.RoundToInt(commandOrientation).ToString());
        else
        {
            tcpclient = gameObject.GetComponent<UDPSend>();
            if (tcpclient) Debug.Log("CONNECTED ");
        }

        //Debug.Log(commandPosition.x.ToString() + " " + commandPosition.y.ToString() + " " + commandPosition.z.ToString() + " " + commandOrientation.ToString());
        Debug.Log(dummyInt.x.ToString() + " " + dummyInt.y.ToString() + " " + dummyInt.z.ToString() + " " + Mathf.RoundToInt(commandOrientation).ToString());
    }

    //  usefull facility function : clamp a vector component-wise
    Vector3 clamp(Vector3 v, Vector3 min, Vector3 max)
    {
        v.x = Mathf.Clamp(v.x, min.x, max.x);
        v.y = Mathf.Clamp(v.y, min.y, max.y);
        v.z = Mathf.Clamp(v.z, min.z, max.z);
        return v;
    }
}