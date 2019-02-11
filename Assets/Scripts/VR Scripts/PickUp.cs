using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PickUp : MonoBehaviour
{

    [SerializeField] private SteamVR_Action_Boolean isGrabbing;

    public Rigidbody attachPoint;

    SteamVR_Behaviour_Pose trackedObj;
    FixedJoint joint;

    private GameObject[] interactables;
    private int i;

    private bool canGrab = false;
    private bool grab = false;
    private bool release = false;

    [SerializeField] private GameObject lCont;
    [SerializeField] private GameObject rCont;

    private void Awake()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
        trackedObj = GetComponent<SteamVR_Behaviour_Pose>();
    }

    private void FixedUpdate()
    {
        grab = isGrabbing.GetStateDown(SteamVR_Input_Sources.Any);
        release = isGrabbing.GetLastStateUp(SteamVR_Input_Sources.Any);

        if (joint == null && grab && canGrab)
        {
            Grab();
            
        }
        else if (joint != null && release)
        {
            interactables[i] = joint.gameObject;
            Rigidbody rigidbody = interactables[i].GetComponent<Rigidbody>();
            Object.DestroyImmediate(joint);
            joint = null;

            Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
            if (origin != null)
            {
                rigidbody.velocity = origin.TransformVector(trackedObj.GetVelocity());
                rigidbody.angularVelocity = origin.TransformVector(trackedObj.GetAngularVelocity());
            }
            else
            {
                rigidbody.velocity = trackedObj.GetVelocity();
                rigidbody.angularVelocity = trackedObj.GetAngularVelocity();
            }

            rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
        }

        if (isGrabbing.GetStateUp(SteamVR_Input_Sources.LeftHand))
        {
            lCont.active = true;
        }
        if (isGrabbing.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            rCont.active = true;
        }

    }

    private void Grab()
    {
            interactables[i].transform.position = attachPoint.transform.position;

            joint = interactables[i].AddComponent<FixedJoint>();
            joint.connectedBody = attachPoint;

            interactables[i].GetComponent<Outline>().enabled = false;
        if (isGrabbing.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            lCont.active = false;
        }
        if (isGrabbing.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            rCont.active = false;
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
