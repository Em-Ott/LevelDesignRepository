using System.Collections;
using UnityEngine;

public class ScreenBlink : MonoBehaviour
{
    [SerializeField] private Material blinkMaterial;
    [SerializeField] private float blinkDuration = 0.25f;

    private static readonly int BlinkProgressID = Shader.PropertyToID("_BlinkProgress");

    public void Start()
    {
        blinkMaterial.SetFloat(BlinkProgressID, 0f);
    }

    public void TriggerBlink()
    {
        StopAllCoroutines();
        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        float halfDuration = blinkDuration * 0.5f;

        // Close eyes
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            blinkMaterial.SetFloat(BlinkProgressID, t / halfDuration);
            yield return null;
        }
        blinkMaterial.SetFloat(BlinkProgressID, 1f);

        // Open eyes
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            blinkMaterial.SetFloat(BlinkProgressID, 1f - (t / halfDuration));
            yield return null;
        }
        blinkMaterial.SetFloat(BlinkProgressID, 0f);
    }
}
