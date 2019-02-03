using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRotation : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("ObjectRotate");
    }

    IEnumerator ObjectRotate()
    {
        float timer = 10000;
        while (true)
        {
            float angle = Mathf.Sin(timer) * 3.7f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
