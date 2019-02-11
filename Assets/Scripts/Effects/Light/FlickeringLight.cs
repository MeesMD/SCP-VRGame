using UnityEngine;
using System.Collections;

public class flickeringLight : MonoBehaviour
{
    private Light pointLight;
    public bool flickerSwitch = false;
    public float minWait = 0.2f;
    public float maxWait = 0.5f;


    void Start()
    {
        pointLight = GetComponent<Light>();
        StartCoroutine(Flickering());
    }

    IEnumerator Flickering()
    {
        while (true)
        {
            if (flickerSwitch)
            {
                yield return new WaitForSeconds(Random.Range(minWait, maxWait));
                pointLight.enabled = !pointLight.enabled;
            }
            else
            {
                yield return null;
                pointLight.enabled = true;
            } 
        }
    }
}