using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactableitem : MonoBehaviour
{
    private bool currentlyInteracting;
    private vivecontroller attachedWand;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (attachedWand && currentlyInteracting)
        {
            
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
            float x = Mathf.Clamp(transform.position.x, -1.5f, 1.5f);
            float y = Mathf.Clamp(transform.position.y,  0.0f, 1.5f);
            float z = Mathf.Clamp(transform.position.z, -1.1f, 1.1f);
            transform.position = new Vector3(x, y, z);
        }
    }

    public void BeginInteraction(vivecontroller wand)
    {
        attachedWand = wand;
        transform.SetParent(wand.transform, true);
        currentlyInteracting = true;
    }

    public void EndInteraction(vivecontroller wand)
    {
        if (wand == attachedWand)
        {
            attachedWand = null;
            currentlyInteracting = false;
            transform.parent = null;
        }
    }

    public bool IsInteracting()
    {
        return currentlyInteracting;
    }
}