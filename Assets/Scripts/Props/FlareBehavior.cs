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
    private float maxDist = 1;


    void Start()
    {
        flarelightScript = flareLight.GetComponent<flareLight>();
        flareEffect.SetActive(false);
    }


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
                flareEffect.SetActive(true);
                StartCoroutine(flarelightScript.Flickering());
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.name);
        if (col.transform.tag == "Wall" && !isLit)
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
