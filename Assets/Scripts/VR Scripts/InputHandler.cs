using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private SteamVR_Action_Vector2 touchpad;
    [SerializeField] private SteamVR_Action_Boolean touchpadPress;

    [SerializeField] private GameObject controller;
    [SerializeField] private GameObject player;

    private Vector2 touchpadValue;
    private double walkMaxTPValue = 0.7f;
    private double walkMinTPValue = 0.7f;
    private bool touchpadPressed = false;

    private float velocity = 2;

    private void Start()
    {

    }

    private void Update()
    {
        touchpadValue = touchpad.GetAxis(SteamVR_Input_Sources.Any);
        touchpadPressed = touchpadPress.GetState(SteamVR_Input_Sources.Any);

        if (touchpadValue.y > walkMaxTPValue && touchpadPressed == true) 
        {
            Move();
        }
        else
        {
            touchpadPressed = false;
            return;
        }
    }

    private void Move()
    {
        Debug.Log("Walk");
        player.GetComponent<Rigidbody>().velocity = controller.transform.forward * velocity;
        return;
    }

}
