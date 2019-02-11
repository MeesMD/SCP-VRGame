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
    [SerializeField] private Rigidbody pRB;

    private Vector2 touchpadValueL;
    private bool touchpadPressedL = false;
    private float walkVelocity = 70f;
    private float runVelocity = 130f;

    private Vector2 touchpadValueR;
    private Quaternion currentRotation;
    private bool touchpadPressedR = false;
    private float tpDir = 0.7f;
    

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

        if (touchpadValueR.x > tpDir && touchpadPressedR)
        {
            //Right
            var Euler = transform.rotation.eulerAngles;
            Euler.y += 45;
            player.transform.rotation = Quaternion.Euler(Euler);
        }
        else if (touchpadValueR.x < -tpDir && touchpadPressedR)
        {
            //Left
            var Euler = transform.rotation.eulerAngles;
            Euler.y -= 45;
            player.transform.rotation = Quaternion.Euler(Euler);
        }
    }

    private void CheckMove()
    {
        if (touchpadValueL.y > tpDir && touchpadPressedL == false)
        {
            Walk();
        }
        else if (touchpadValueL.y > tpDir && touchpadPressedL == true)
        {
            Run();
        }
        else
        {
            pRB.velocity = Vector3.zero;
            touchpadPressedL = false;
            return;
        }
    }

    private void Walk()
    {
        //Debug.Log("Walk");
        pRB.velocity = controller.transform.forward * walkVelocity * Time.deltaTime;
        return;
    }

    private void Run()
    {
        //Debug.Log("Run");
        pRB.velocity = controller.transform.forward * runVelocity * Time.deltaTime;
        return;
    }

}
