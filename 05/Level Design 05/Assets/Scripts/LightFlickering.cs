using UnityEngine;
using System.Collections;

public class LightFlickering : MonoBehaviour
{
    private Light light;

    private float min = 0.3f;
    private float max = 1.0f;
    private float intensity = 0.1f;

    private bool flickering;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light = GetComponent<Light>();
        StartCoroutine(Lights());
        flickering = true;
    }

    void Update() {

    }

    public void ChangeLight(float newMin, float newMax, float newIntensity) {
        if (newMin == 10.0f) {
            flickering = false;
            light.intensity = newIntensity;
        } else {
            min = newMin;
            max = newMax;
            intensity = newIntensity;
        }
    }

    // Update is called once per frame
    IEnumerator Lights()
    {
        light.intensity = intensity;
        light.enabled = !light.enabled;
        yield return new WaitForSeconds (Random.Range(min, max));
        if (flickering) {
            StartCoroutine(Lights());
        }
    }
}
