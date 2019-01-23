using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Movement : MonoBehaviour
{

    [SerializeField]private SteamVR_Action_Vector2 touchPadAction;
    [SerializeField]private SteamVR_Action_Boolean trigger;
    
    private Vector2 touchPadValue;
    private bool triggerValue;

    void Update()
    {
        touchPadValue = touchPadAction.GetAxis(SteamVR_Input_Sources.Any);
        triggerValue = trigger.GetActive(SteamVR_Input_Sources.Any);

        if(touchPadValue != Vector2.zero)
        {
            Move();
        }
    }

    private void Move()
    {
        print(touchPadValue);
    }
}
