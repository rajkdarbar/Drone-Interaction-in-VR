using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vivecontroller : MonoBehaviour {
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip; 
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;   

    private SteamVR_TrackedObject trackedObj;
    private GameObject pickup;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    HashSet<interactableitem> objectsHoveringOver = new HashSet<interactableitem>();

    private interactableitem closestItem;
    private interactableitem interactingItem;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if(controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        /*
        if (controller.GetPressDown(gripButton))
        {
            Debug.Log("Grip Button was just pressed.");
        }

        if (controller.GetPressUp(gripButton))
        {
            Debug.Log("Grip Button was just unpressed.");
        }
        

        if (controller.GetPressDown(triggerButton) && pickup != null)
        {
            //Debug.Log("Trigger Button was just pressed.");
            pickup.transform.parent = this.transform;
            pickup.GetComponent<Rigidbody>().useGravity = false;
        }

        if (controller.GetPressUp(triggerButton) && pickup != null)
        {
            //Debug.Log("Trigger Button was just unpressed.");
            pickup.transform.parent = null;
            pickup.GetComponent<Rigidbody>().useGravity = true;
        }
        */

        if (controller.GetPressDown(triggerButton))
        {
            closestItem = null;
            float minDistance = float.MaxValue;
            float distance;
            foreach(interactableitem item in objectsHoveringOver)
            {
                distance = (item.transform.position - transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = item;
                }  
            }

            interactingItem = closestItem;
            if (interactingItem)
            {
                if (interactingItem.IsInteracting())
                {
                    interactingItem.EndInteraction(this);
                }
                interactingItem.BeginInteraction(this);
            }
        }

        if (controller.GetPressUp(triggerButton) && interactingItem != null)
        {
            interactingItem.EndInteraction(this);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("Trigger entered");
        //pickup = collider.gameObject;
        interactableitem collidedItem = collider.GetComponent<interactableitem>();
        if (collidedItem)
        {
            objectsHoveringOver.Add(collidedItem);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        //Debug.Log("Trigger stay");
    }

    private void OnTriggerExit(Collider collider)
    {
        //Debug.Log("Trigger exit");
        //pickup = null;
        interactableitem collidedItem = collider.GetComponent<interactableitem>();
        if (collidedItem)
        {
            objectsHoveringOver.Remove(collidedItem);
        }
    }
}