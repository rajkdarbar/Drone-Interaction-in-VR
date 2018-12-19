using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap.Unity.Interaction;

public class KeyboardInteraction : MonoBehaviour {

    public pid_button_selection drone;
    public TextMesh target_btn, clicked_btn; 
    private int key;

    void Start () {
        key = 1;
        target_btn = GameObject.Find("target_button").transform.GetChild(0).GetComponent<TextMesh>();
        target_btn.text = key.ToString();

        clicked_btn = GameObject.Find("clicked_button").transform.GetChild(0).GetComponent<TextMesh>();

        drone.goal = searchTargetByKey(key);
    }

	void Update () {
        
    }

    public void childClick(GameObject child)
    {
        clicked_btn.text = child.transform.Find("Text").GetComponent<TextMesh>().text;

        if(child.transform.Find("Text").GetComponent<TextMesh>().text == key.ToString())
        {
            key = Random.Range(1, 9);            
            drone.goal = searchTargetByKey(key);
        }
    }

    private GameObject searchTargetByKey(int k)
    {
        return transform.Find("Key" + k.ToString()).gameObject.transform.Find("GoalTarget").gameObject;
    }
}