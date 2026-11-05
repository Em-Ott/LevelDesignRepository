using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] public AudioSource SFXSource;
    [SerializeField] private SoundEmitter _defaultEmitter;

    private void Awake()
    {
        if (SFXSource == null)
        {
            SFXSource = GetComponentInChildren<AudioSource>();
        }

        if (_defaultEmitter == null)
        {
            _defaultEmitter = GetComponent<SoundEmitter>();
        }

        if (_defaultEmitter == null)
        {
            _defaultEmitter = gameObject.AddComponent<SoundEmitter>();
            _defaultEmitter.Spatial = false;
        }
    }

    public void Play(SoundCue cue, double dspTime)
    {
        SoundEmitter emitter = cue.Emitter != null ? cue.Emitter : _defaultEmitter;
        emitter.Schedule(cue.Clip, cue.Volume, dspTime, SFXSource);
        Debug.Log($"[SFXManager] - clip name - {cue.Clip.name} at {cue.Time:0.00}s");
    }
}
