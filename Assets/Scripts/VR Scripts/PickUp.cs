using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PickUp : MonoBehaviour
{

    [SerializeField] private SteamVR_Action_Boolean isGrabbing;


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Interactable")
        {
            Debug.Log("Is colliding");
        }
    }
}
