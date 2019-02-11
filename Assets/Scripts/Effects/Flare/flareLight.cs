using UnityEngine;
using System.Collections;

public class flareLight : MonoBehaviour
{
    private Light pointLight;
    public float minWait = 0.05f;
    public float maxWait = 0.09f;


    void Start()
    {
        pointLight = GetComponent<Light>();
    }

    public IEnumerator Flickering()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
            pointLight.intensity = (Random.Range(0.5f, 1f));
        }
    }
}