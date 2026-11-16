using System.Collections;
using UnityEngine;

public class VoiceOverManager : MonoBehaviour
{
    [SerializeField] public AudioSource VCSource;
    [SerializeField] private SoundEmitter _defaultEmitter;
    [SerializeField] private bool _showSubtitles = true;
    [SerializeField] private Color _victimColor = new Color(0.86f, 0.85f, 0.8f, 1f);
    [SerializeField] private Color _killerColor = new Color(0.82f, 0.2f, 0.16f, 1f);
    [SerializeField] private float _subtitleTailTrim = 1.2f;

    private SoundsManager _manager;

    private void Awake()
    {
        _manager = GetComponentInParent<SoundsManager>();

        if (VCSource == null)
        {
            VCSource = GetComponentInChildren<AudioSource>();
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

    public void Play(SoundCue cue, double dspTime, SequenceEffects effects)
    {
        SoundEmitter emitter = cue.Emitter != null ? cue.Emitter : _defaultEmitter;
        emitter.Schedule(cue.Clip, cue.Volume, dspTime, VCSource);
        Debug.Log($"[VoiceOverManager] - clip name - {cue.Clip.name} at {cue.Time:0.00}s");

        if (_showSubtitles && effects != null && !string.IsNullOrEmpty(cue.Subtitle))
        {
            StartCoroutine(ShowSubtitle(cue, dspTime, effects));
        }
    }

    private IEnumerator ShowSubtitle(SoundCue cue, double dspTime, SequenceEffects effects)
    {
        while (_manager != null ? !_manager.Started || _manager.SequenceTime < cue.Time : AudioSettings.dspTime < dspTime)
        {
            yield return null;
        }

        Color color = cue.Speaker == SubtitleSpeaker.Killer ? _killerColor : _victimColor;
        float duration = Mathf.Max(1.5f, cue.Clip.length - _subtitleTailTrim);
        effects.ShowSubtitle(cue.Subtitle, color, duration);
    }
}
