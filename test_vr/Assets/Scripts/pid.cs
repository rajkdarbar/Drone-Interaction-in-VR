using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class pid : MonoBehaviour
{
    public GameObject goal;
    private UDPSend udpclient;
    
    public float elapseTime = 0.016f; // 16msec per frame ~ 62FPS

    private Vector3 integralPositionError;
    private Vector3 previousPositionError;
    private float integralOrientationError;
    private float previousOrientationError;

    [Header("Proportionnal gains :")]
    public float Kp_pitchroll;
    public float Kp_vertical;
    public float Kp_yaw;

    [Header("Integral gains :")]
    public float Ki_pitchroll;
    public float Ki_vertical;
    public float Ki_yaw;

    [Header("Derivative gains :")]
    public float Kd_pitchroll;
    public float Kd_vertical;
    public float Kd_yaw;

    [Header("Command clamping values :")]
    public float CommandClamp_pitchroll;
    public float CommandClamp_vertical;
    public float CommandClamp_yaw;

    [Header("Integral clamping values :")]
    public float IntegralClamp_pitchroll;
    public float IntegralClamp_vertical;
    public float IntegralClamp_yaw;

    private Vector3 previousCommand;

    // Use this for initialization
    void Start()
    {
        // initialize PID parameters
        integralPositionError = new Vector3(0, 0, 0);
        previousPositionError = new Vector3(0, 0, 0);
        integralOrientationError = 0.0f;
        previousOrientationError = 0.0f;

        // get raspberry pi socket
        udpclient = gameObject.GetComponent<UDPSend>();
    }

    //  PID
    void Update()
    {
        //  compute errors
        Vector3 positionError = goal.transform.position - transform.position;
        Vector3 derivativeError = (positionError - previousPositionError) / elapseTime;

        float angle = goal.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;
        if (goal.GetComponent<interactableitem>().IsInteracting())   // goal is in interaction mode, so drone dont follow goal orientation
            angle = 0.0f;
        
        //  keep angle between -180 and 180 degree
        if (angle > 180) angle -= 360.0f;                           
        else if (angle < -180) angle += 360.0f;                     
        float orientationError = Mathf.Deg2Rad * angle;
        float derivativeOrientationError = (orientationError - previousOrientationError) / elapseTime;
        

        //  compute PID
        Vector3 commandPosition = new Vector3();
        commandPosition.x = Kp_pitchroll * positionError.x + Ki_pitchroll * integralPositionError.x + Kd_pitchroll * derivativeError.x;
        commandPosition.y = Kp_vertical *  positionError.y + Ki_vertical *  integralPositionError.y + Kd_vertical *  derivativeError.y;
        commandPosition.z = Kp_pitchroll * positionError.z + Ki_pitchroll * integralPositionError.z + Kd_pitchroll * derivativeError.z;
        
        float commandOrientation = Kp_yaw * orientationError + Ki_yaw * integralOrientationError + Kd_yaw * derivativeOrientationError;


        // update PID parameters
        previousPositionError = positionError;
        integralPositionError += elapseTime * positionError;

        previousOrientationError = orientationError;
        integralOrientationError += elapseTime * orientationError;
        

        // clamp commands and integral error to prevent drone dammages
        commandPosition.x = Mathf.Clamp(commandPosition.x, -CommandClamp_pitchroll, CommandClamp_pitchroll);
        commandPosition.y = Mathf.Clamp(commandPosition.y, -CommandClamp_vertical, CommandClamp_vertical);
        commandPosition.z = Mathf.Clamp(commandPosition.z, -CommandClamp_pitchroll, CommandClamp_pitchroll);
        commandOrientation = Mathf.Clamp(commandOrientation, -CommandClamp_yaw, CommandClamp_yaw);

        integralPositionError.x = Mathf.Clamp(integralPositionError.x, -IntegralClamp_pitchroll, IntegralClamp_pitchroll);
        integralPositionError.y = Mathf.Clamp(integralPositionError.y, -IntegralClamp_vertical, IntegralClamp_vertical);
        integralPositionError.z = Mathf.Clamp(integralPositionError.z, -IntegralClamp_pitchroll, IntegralClamp_pitchroll);
        integralOrientationError = Mathf.Clamp(integralOrientationError, -IntegralClamp_yaw, IntegralClamp_yaw);


        //  compute local command : from global Optitrack to local drone body
        Matrix4x4 localToWorld = Matrix4x4.Translate(transform.position) * Matrix4x4.Rotate(transform.rotation);
        Vector4 localPositionCommand = localToWorld.inverse * new Vector4(commandPosition.x, commandPosition.y, commandPosition.z, 0.0f);
        Vector3Int msgComm = new Vector3Int(Mathf.RoundToInt(localPositionCommand.x), Mathf.RoundToInt(localPositionCommand.y), Mathf.RoundToInt(localPositionCommand.z));


        //  Send command to raspberry pi : to make sure the order of execution doesnt matter (unity <-> raspberry pi)
        if (udpclient)
        {
            udpclient.SendMessage(msgComm.x.ToString() + " " + msgComm.y.ToString() + " " + msgComm.z.ToString() + " " + Mathf.RoundToInt(commandOrientation).ToString());
        }
        else
        {
            udpclient = gameObject.GetComponent<UDPSend>();
            if (udpclient) Debug.Log("CONNECTED ");
        }

        //Debug.Log(localPositionCommand.x.ToString() + " " + localPositionCommand.y.ToString() + " " + localPositionCommand.z.ToString() + " " + Mathf.RoundToInt(commandOrientation).ToString());
        //Debug.Log(msgComm.x.ToString() + " " + msgComm.y.ToString() + " " + msgComm.z.ToString() + " " + Mathf.RoundToInt(commandOrientation).ToString());
    }
}