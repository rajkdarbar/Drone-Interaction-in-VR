using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class pid : MonoBehaviour
{
    public GameObject goal;
    private UDPSend tcpclient;
    
    public float elapseTime = 0.016f;

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
        
        Vector3 commandPosition = new Vector3();
            commandPosition.x = Kp_pitchroll * positionError.x + Ki_pitchroll * integralPositionError.x + Kd_pitchroll * derivativeError.x;
            commandPosition.y = Kp_vertical *  positionError.y + Ki_vertical *  integralPositionError.y + Kd_vertical *  derivativeError.y;
            commandPosition.z = Kp_pitchroll * positionError.z + Ki_pitchroll * integralPositionError.z + Kd_pitchroll * derivativeError.z;
        
        float angle = goal.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;
        if (goal.GetComponent<interactableitem>().IsInteracting())
            angle = 0.0f;
        if (angle > 180) angle -= 360.0f;
        else if (angle < -180) angle += 360.0f;
        float orientationError = Mathf.Deg2Rad * angle;
        float derivativeOrientationError = (orientationError - previousOrientationError) / elapseTime;
        float commandOrientation = Kp_yaw * orientationError + Ki_yaw * integralOrientationError + Kd_yaw * derivativeOrientationError;

        // shifting corrector parameters
        previousPositionError = positionError;
        integralPositionError += elapseTime * positionError;

        previousOrientationError = orientationError;
        integralOrientationError += elapseTime * orientationError;
        previousCommand = commandPosition;
        
        // clamp command and integral error to prevent drone dammages
        //float maxCommandMagnitude = 40.0f;
        //float maxCommandMagnitudePitchRoll = 20.0f;
        //float maxIntegralErrorMagnitude = 1.0f;

        //commandPosition = clamp(commandPosition, new Vector3(-maxCommandMagnitudePitchRoll, -maxCommandMagnitude, -maxCommandMagnitudePitchRoll), new Vector3(maxCommandMagnitudePitchRoll, maxCommandMagnitude, maxCommandMagnitudePitchRoll));
        //integralPositionError = clamp(integralPositionError, new Vector3(-maxIntegralErrorMagnitude, -maxIntegralErrorMagnitude, -maxIntegralErrorMagnitude), new Vector3(maxIntegralErrorMagnitude, maxIntegralErrorMagnitude, maxIntegralErrorMagnitude));

        commandPosition.x = Mathf.Clamp(commandPosition.x, -CommandClamp_pitchroll, CommandClamp_pitchroll);
        commandPosition.y = Mathf.Clamp(commandPosition.y, -CommandClamp_vertical, CommandClamp_vertical);
        commandPosition.z = Mathf.Clamp(commandPosition.z, -CommandClamp_pitchroll, CommandClamp_pitchroll);
        commandOrientation = Mathf.Clamp(commandOrientation, -CommandClamp_yaw, CommandClamp_yaw);

        integralPositionError.x = Mathf.Clamp(integralPositionError.x, -IntegralClamp_pitchroll, IntegralClamp_pitchroll);
        integralPositionError.y = Mathf.Clamp(integralPositionError.y, -IntegralClamp_vertical, IntegralClamp_vertical);
        integralPositionError.z = Mathf.Clamp(integralPositionError.z, -IntegralClamp_pitchroll, IntegralClamp_pitchroll);
        integralOrientationError = Mathf.Clamp(integralOrientationError, -IntegralClamp_yaw, IntegralClamp_yaw);

        //  compute local command
        Matrix4x4 localToWorld = Matrix4x4.Translate(transform.position) * Matrix4x4.Rotate(transform.rotation);
        Vector4 localPositionCommand = localToWorld.inverse * new Vector4(commandPosition.x, commandPosition.y, commandPosition.z, 0.0f);
        Vector3Int msgComm = new Vector3Int(Mathf.RoundToInt(localPositionCommand.x), Mathf.RoundToInt(localPositionCommand.y), Mathf.RoundToInt(localPositionCommand.z));

        //  Send Command to NodeJS server
        if (tcpclient)
        {
            // python unpack it this way [-pitch, vertical, roll, yaw]
            tcpclient.SendMessage(msgComm.x.ToString() + " " + msgComm.y.ToString() + " " + msgComm.z.ToString() + " " + Mathf.RoundToInt(commandOrientation).ToString());
        }
        else
        {
            tcpclient = gameObject.GetComponent<UDPSend>();
            if (tcpclient) Debug.Log("CONNECTED ");
        }

        //Debug.Log(localPositionCommand.x.ToString() + " " + localPositionCommand.y.ToString() + " " + localPositionCommand.z.ToString() + " " + Mathf.RoundToInt(commandOrientation).ToString());
        //Debug.Log(msgComm.x.ToString() + " " + msgComm.y.ToString() + " " + msgComm.z.ToString() + " " + Mathf.RoundToInt(commandOrientation).ToString());
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