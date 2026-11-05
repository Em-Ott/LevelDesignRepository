using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] VoiceOverManager VCManager;
    [SerializeField] SFXManager SFXManager;

    void Start()
    {
        Debug.Log($"[SoundsManager] - start before checks");

        if (VCManager == null)
        {
            VCManager = GetComponentInChildren<VoiceOverManager>();
        }

        if (SFXManager == null)
        {
            SFXManager = GetComponentInChildren<SFXManager>();
        }

        Debug.Log($"[SoundsManager] - start after checks");


        StartCoroutine(FirstSFXThenCoroutine());
        Debug.Log($"[SoundsManager] - after coroutine call");

    }

    private IEnumerator FirstSFXThenCoroutine()
    {
        Debug.Log($"[SoundsManager] - first call");
        PlaySFX();

        yield return new WaitForSeconds(3f);

        PlayVoices();
    }

    public void PlayVoices()
    {
        StartCoroutine(PlayVoiceRoutine());
    }

    private IEnumerator PlayVoiceRoutine()
    {
        if (!VCManager.Finished)
        {
            VCManager.PlayClip();

            // Wait until the audio source is no longer playing
            while (VCManager.VCSource.isPlaying)
            {
                yield return null;
            }

            // Action to perform when the clip finishes
            Debug.Log("Voice Audio is done playing!");
            PlayVoices();
        }
    }


    public void PlaySFX()
    {
        StartCoroutine(PlaySFXRoutine());
    }

    private IEnumerator PlaySFXRoutine()
    {
        if (!SFXManager.Finished)
        {
            SFXManager.PlayClip();

            // Wait until the audio source is no longer playing
            while (SFXManager.SFXSource.isPlaying)
            {
                yield return null;
            }

            // Action to perform when the clip finishes
            Debug.Log("SFX Audio is done playing!");
            PlaySFX();
        }
    }

}
