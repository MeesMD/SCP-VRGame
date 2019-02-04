using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour
{
    private Light pointLight;
    public bool flickerSwitch = false;
    public float minWait = 0.5f;
    public float maxWait = 0.2f;


    void Start()
    {
        pointLight = GetComponent<Light>();
    }

    void Update()
    {
        if (flickerSwitch)
        {
            StartCoroutine(Flickering());
        }
        else
        {
            StopCoroutine(Flickering());
            pointLight.enabled = true;
        }
    }

    IEnumerator Flickering()
    { 
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        pointLight.enabled = !pointLight.enabled;
    }
}