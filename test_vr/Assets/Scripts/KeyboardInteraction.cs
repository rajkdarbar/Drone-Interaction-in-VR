using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap.Unity.Interaction;

public class KeyboardInteraction : MonoBehaviour {

    public pid_button_selection drone;
    public TextMesh targetKey, enteredText;
    private int key;

    void Start () {
        key = 1;
        targetKey.text = key.ToString();
        drone.goal = searchTargetByKey(key);
    }

	void Update () {
        
    }

    public void childClick(GameObject child)
    {
        enteredText.text += child.transform.Find("Text").GetComponent<TextMesh>().text;

        if(child.transform.Find("Text").GetComponent<TextMesh>().text == key.ToString())
        {
            key = Random.Range(1, 9);
            targetKey.text = key.ToString();
            drone.goal = searchTargetByKey(key);
        }
    }

    private GameObject searchTargetByKey(int k)
    {
        return transform.Find("Key" + k.ToString()).gameObject.transform.Find("GoalTarget").gameObject;
    }
}