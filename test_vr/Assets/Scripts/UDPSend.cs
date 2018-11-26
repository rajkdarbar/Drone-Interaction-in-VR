using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class UDPSend : MonoBehaviour
{
    private static int localPort;

    // prefs
    public string IP; // define in init
    public int port; // define in init

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // start from unity3d
    public void Start()
    {
        init();
    }


    // init
    public void init()
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SendMessage("takeoff");
            Debug.Log("takeoff");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SendMessage("landing");
            Debug.Log("landing");
        }

        //---------------------pitch--------------------------------------------
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SendMessage("up_key_pressed");
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            SendMessage("up_key_unpressed");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SendMessage("down_key_pressed");
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            SendMessage("down_key_unpressed");
        }

        //---------------------roll--------------------------------------------
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SendMessage("right_key_pressed");
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            SendMessage("right_key_unpressed");
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SendMessage("left_key_pressed");
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            SendMessage("left_key_unpressed");
        }

        //---------------------altitude--------------------------------------------
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SendMessage("z_key_pressed");
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            SendMessage("z_key_unpressed");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SendMessage("x_key_pressed");
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            SendMessage("x_key_unpressed");
        }

        //---------------------yaw--------------------------------------------
        if (Input.GetKeyDown(KeyCode.R))
        {
            SendMessage("r_key_pressed");
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            SendMessage("r_key_unpressed");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SendMessage("t_key_pressed");
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            SendMessage("t_key_unpressed");
        }
    }

    // sendData
    public void SendMessage(string message)
    {
        try
        {
            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);
            
            // Den message zum Remote-Client senden.
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }   

    // endless test
}
