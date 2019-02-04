using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private SteamVR_Action_Vector2 touchpad;
    [SerializeField] private SteamVR_Action_Boolean touchpadPressL;
    [SerializeField] private SteamVR_Action_Boolean touchpadPressR;

    [SerializeField] private GameObject controller;
    [SerializeField] private GameObject player;

    private Vector2 touchpadValueL;
    private bool touchpadPressedL = false;
    private float walkVelocity = 1;
    private float runVelocity = 2.3f;

    private Vector2 touchpadValueR;
    private Quaternion currentRotation;
    private bool touchpadPressedR = false;
    private float tpDir = 0.7f;


    private void Start()
    {

    }

    private void Update()
    {
        touchpadValueL = touchpad.GetAxis(SteamVR_Input_Sources.LeftHand);
        touchpadPressedL = touchpadPressL.GetState(SteamVR_Input_Sources.LeftHand);

        touchpadValueR = touchpad.GetAxis(SteamVR_Input_Sources.RightHand);
        touchpadPressedR = touchpadPressR.GetStateUp(SteamVR_Input_Sources.RightHand);

        CheckMove();
        Rotate();

    }

    private void Rotate()
    {
        currentRotation = player.transform.rotation;
        
        if(touchpadValueR.y < -tpDir && touchpadPressedR)
        {
            //Down
            player.transform.Rotate(0, 180, 0);
        }

        if (touchpadValueR.x > tpDir && touchpadPressedR)
        {
            //Right
            player.transform.Rotate(0, 90, 0);
        }
        else if (touchpadValueR.x < -tpDir && touchpadPressedR)
        {
            //Left
            player.transform.Rotate(0, 270, 0);
        }
    }

    private void CheckMove()
    {
        if (touchpadValueL.y > tpDir && touchpadPressedL == false)
        {
            Walk();
        }

        if (touchpadValueL.y > tpDir && touchpadPressedL == true)
        {
            Run();
        }
        else
        {
            touchpadPressedL = false;
            return;
        }
    }

    private void Walk()
    {
        Debug.Log("Walk");
        player.GetComponent<Rigidbody>().velocity = controller.transform.forward * walkVelocity;
        return;
    }

    private void Run()
    {
        Debug.Log("Run");
        player.GetComponent<Rigidbody>().velocity = controller.transform.forward * runVelocity;
        return;
    }

}
