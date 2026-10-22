using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightFlickering : MonoBehaviour
{
    [SerializeField] private Volume volume;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private ShadowsMidtonesHighlights shadowsMidtonesHighlights;
    private FilmGrain filmGrain;
    private ChromaticAberration aberrations;
    private SplitToning splitToning;

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


    public void ChangeLight(float newMin, float newMax, float newIntensity)
    {
        if (newMin == 10.0f)
        {
            flickering = false;
            light.intensity = newIntensity;
            light.enabled = true;
            SetNormal();
        }
        else
        {
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
        yield return new WaitForSeconds(Random.Range(min, max));
        if (flickering)
        {
            StartCoroutine(Lights());
        }
    }

    private void SetNormal()
    {
        if (volume.profile.TryGet(out Vignette v))
            v.intensity.Override(0f);

        if (volume.profile.TryGet(out FilmGrain fg))
            fg.intensity.Override(0f);

        if (volume.profile.TryGet(out ChromaticAberration ca))
            ca.intensity.Override(0f);

        if (volume.profile.TryGet(out ColorAdjustments c))
        {
            c.postExposure.Override(0f);
            c.contrast.Override(0f);
            c.saturation.Override(0f);
            c.hueShift.Override(0f);
            c.colorFilter.Override(Color.white);
        }

        if (volume.profile.TryGet(out ShadowsMidtonesHighlights smh))
        {
            smh.shadows.Override(new Vector4(1f, 1f, 1f, 0f));
            smh.midtones.Override(new Vector4(1f, 1f, 1f, 0f));
            smh.highlights.Override(new Vector4(1f, 1f, 1f, 0f));
        }

        if (volume.profile.TryGet(out SplitToning st))
        {
            st.shadows.Override(Color.gray);
            st.highlights.Override(Color.gray);
            st.balance.Override(0f);
        }
    }

}
