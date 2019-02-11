using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PickUp : MonoBehaviour
{

    [SerializeField] private SteamVR_Action_Boolean isGrabbing;

    private GameObject[] interactables;
    private int i;

    private bool canGrab = false;
    private bool lGrab = false;
    private bool rGrab = false;

    [SerializeField] private GameObject lCont;
    [SerializeField] private GameObject rCont;

    private void Awake()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
    }

    private void FixedUpdate()
    {
        lGrab = isGrabbing.GetState(SteamVR_Input_Sources.LeftHand);
        rGrab = isGrabbing.GetState(SteamVR_Input_Sources.RightHand);

        Grab();
    }

    private void Grab()
    {
        if(canGrab && lGrab)
        {
            interactables[i].transform.localRotation = lCont.transform.rotation;
            interactables[i].transform.localPosition = lCont.transform.position;
        }

        if (canGrab && rGrab)
        {
            interactables[i].transform.localRotation = rCont.transform.rotation;
            interactables[i].GetComponent<Rigidbody>().velocity = (rCont.transform.position - rCont.transform.position) / Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Interactable")
        {
            interactables[i].GetComponent<Outline>().enabled = true;
            canGrab = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interactable")
        {
            interactables[i].GetComponent<Outline>().enabled = false;
            canGrab = false;
        }
    }
}
