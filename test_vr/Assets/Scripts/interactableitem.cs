using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactableitem : MonoBehaviour
{
    private bool currentlyInteracting;
    private vivecontroller attachedWand;
    private Transform previousParent;

    // Use this for initialization
    void Start()
    {
        previousParent = this.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedWand && currentlyInteracting)
        {
            // in interaction mode, do nothing
        }
        else
        {
            // not interacting : keep the target goal on interaction space and avoid rotation on X and Z axis
            transform.localEulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
            float x = Mathf.Clamp(transform.position.x, -1.5f, 1.5f);
            float y = Mathf.Clamp(transform.position.y,  0.0f, 1.5f);
            float z = Mathf.Clamp(transform.position.z, -1.1f, 1.1f);
            transform.position = new Vector3(x, y, z);
        }
    }

    public void BeginInteraction(vivecontroller wand)
    {
        // the goal game object become child of wand game object
        attachedWand = wand;
        previousParent = this.transform.parent;
        transform.SetParent(wand.transform, true);
        currentlyInteracting = true;
    }

    public void EndInteraction(vivecontroller wand)
    {
        if (wand == attachedWand)
        {
            // detach goal game object to wand game object
            attachedWand = null;
            currentlyInteracting = false;
            this.transform.parent = previousParent;
        }
    }

    public bool IsInteracting()
    {
        return currentlyInteracting;
    }
}