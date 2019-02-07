﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareBehavior : MonoBehaviour
{

    private Vector3 TouchedWallPos;
    [SerializeField]
    private GameObject flareEffect;
    private bool isLit;
    private bool touchWall;
    [SerializeField]
    private float maxDist = 2;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(TouchedWallPos);

        if (touchWall & !isLit)
        {
            float distance = Vector3.Distance(TouchedWallPos, transform.position);
            if(distance > maxDist)
            {
                //ACTIVATE FLARE
                Debug.Log("PFSFEFEHEHEFHEF");
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.name);
        if(col.transform.tag == "Wall" && !isLit)
        {
            Debug.Log("Touched Wall");
            touchWall = true;
            TouchedWallPos = transform.position;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Wall")
        {
            touchWall = false;
        }
    }
}