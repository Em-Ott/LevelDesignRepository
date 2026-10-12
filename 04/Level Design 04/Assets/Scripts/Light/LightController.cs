using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    [SerializeField] private Light sunlight;
    [SerializeField] private GameObject nuke;
    [SerializeField] private Volume volume;
    [SerializeField] private float elaspeSpeed;
    [SerializeField] private bool isDoingCircle = true;
    [SerializeField] private float initialRotation;
    [SerializeField] private float bigBangThreshold;

    [Header("BigBang Bloom")]
    [SerializeField] private float bigBangBloomIntensity = 5f;
    [SerializeField] private float bigBangBloomScatter = 1f;
    [SerializeField] private float bloomRiseDuration = 0.2f;
    [SerializeField] private float bloomTargetIntensity = 1f;
    [SerializeField] private float bloomTargetScatter = 0f;
    [SerializeField] private float bloomRecoveryDuration = 2f;

    private float rotationElapsed;
    private bool isNuked;
    private Bloom bloom;

    void Start()
    {
        sunlight.transform.rotation = Quaternion.Euler(initialRotation, 0f, 0f);

        volume.profile.TryGet(out bloom);
    }

    void Update()
    {
        if (isDoingCircle)
        {
            float step = Time.deltaTime * elaspeSpeed;
            sunlight.transform.Rotate(Vector3.right, step);
            rotationElapsed += step;
        }

        if (!isNuked && (rotationElapsed + initialRotation) > bigBangThreshold)
        {
            isDoingCircle = false;
            sunlight.transform.rotation = Quaternion.Euler(bigBangThreshold, 0f, 0f);
            BigBang();
        }
    }

    private void BigBang()
    {
        isNuked = true;

        nuke.SetActive(true);
        bloom.intensity.overrideState = true;
        bloom.scatter.overrideState = true;
        StartCoroutine(AnimateBloom());
    }

    private IEnumerator AnimateBloom()
    {
        float startIntensity = bloom.intensity.value;
        float startScatter = bloom.scatter.value;

        for (float elapsed = 0f; elapsed < bloomRiseDuration; elapsed += Time.deltaTime)
        {
            float progress = Mathf.SmoothStep(0f, 1f, elapsed / bloomRiseDuration);
            bloom.intensity.value = Mathf.Lerp(startIntensity, bigBangBloomIntensity, progress);
            bloom.scatter.value = Mathf.Lerp(startScatter, bigBangBloomScatter, progress);
            yield return null;
        }

        bloom.intensity.value = bigBangBloomIntensity;
        bloom.scatter.value = bigBangBloomScatter;

        while (true)
        {
            float decay = Mathf.Exp(-Time.deltaTime / bloomRecoveryDuration);
            float intensity = Mathf.Lerp(bloomTargetIntensity, bloom.intensity.value, decay);
            float scatter = Mathf.Lerp(bloomTargetScatter, bloom.scatter.value, decay);

            if (intensity != bloomTargetIntensity)
                bloom.intensity.value = intensity;

            if (scatter != bloomTargetScatter)
                bloom.scatter.value = scatter;

            yield return null;
        }
    }
}
