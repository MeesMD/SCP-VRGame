using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareBehavior : MonoBehaviour
{

    public GameObject flareLight;
    public GameObject flareEffect;
    private flareLight flarelightScript;
    private Vector3 TouchedWallPos;
    private bool isLit;
    private bool touchWall;
    private float maxDist = 2;
    private float rayDist = 0.5f;


    void Start()
    {
        flarelightScript = flareLight.GetComponent<flareLight>();
        flareEffect.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (touchWall & !isLit || Input.GetKeyDown("space"))
        {
            float distance = Vector3.Distance(TouchedWallPos, transform.position);
            if(distance > maxDist)
            {
                //ACTIVATE FLARE
                Debug.Log("PFSFEFEHEHEFHEF");
                flareEffect.SetActive(true);
                StartCoroutine(flarelightScript.Flickering());
            }
        }
    }
}
