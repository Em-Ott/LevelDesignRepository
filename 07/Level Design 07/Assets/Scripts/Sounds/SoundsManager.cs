using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public enum SoundChannel { Ambience, SFX, Voice, Score }

public enum SubtitleSpeaker { None, Victim, Killer }

public enum SequenceEventType { Shake, Flicker, Blackout, End }

[Serializable]
public class SoundCue
{
    public string Name;
    public float Time;
    public AudioClip Clip;
    public SoundChannel Channel;
    public SoundEmitter Emitter;
    [Range(0f, 1f)]
    public float Volume = 1f;
    public SubtitleSpeaker Speaker;
    public string Subtitle;
}

[Serializable]
public class SequenceEvent
{
    public string Name;
    public float Time;
    public SequenceEventType Type;
    public float Duration;
    public float Strength;
    public int Seed;
}

public class SoundsManager : MonoBehaviour
{
    [SerializeField] VoiceOverManager VCManager;
    [SerializeField] SFXManager SFXManager;
    [SerializeField] private SequenceEffects _effects;
    [SerializeField] private float _startDelay = 1.5f;
    [SerializeField] private bool _waitForInputOnWeb = true;
    [SerializeField] private float _scheduleAhead = 1f;
    [SerializeField] private Key _pauseKey = Key.B;
    [SerializeField] private List<SoundCue> _cues = new List<SoundCue>();
    [SerializeField] private List<SequenceEvent> _events = new List<SequenceEvent>();

    private SoundEmitter[] _emitters;
    private double _startDspTime;
    private bool _started;
    private bool _paused;
    private double _pauseDspTime;
    private float _timeScaleBeforePause = 1f;

    public bool Started => _started;
    public bool Paused => _paused;
    public bool Finished { get; private set; }
    public float SequenceTime => _started ? (float)((_paused ? _pauseDspTime : AudioSettings.dspTime) - _startDspTime) : 0f;

    void Start()
    {
        if (VCManager == null)
        {
            VCManager = GetComponentInChildren<VoiceOverManager>();
        }

        if (SFXManager == null)
        {
            SFXManager = GetComponentInChildren<SFXManager>();
        }

        if (_effects == null)
        {
            _effects = GetComponentInChildren<SequenceEffects>();
        }

        _emitters = GetComponentsInChildren<SoundEmitter>();
        foreach (SoundEmitter emitter in _emitters)
        {
            emitter.Follow(0f);
        }

        StartCoroutine(PlaySequence());
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && _pauseKey != Key.None && keyboard[_pauseKey].wasPressedThisFrame)
        {
            SetPaused(!_paused);
        }

        if (!_started)
        {
            return;
        }

        float time = SequenceTime;
        foreach (SoundEmitter emitter in _emitters)
        {
            emitter.Follow(time);
        }
    }

    public void SetPaused(bool paused)
    {
        if (paused == _paused)
        {
            return;
        }

        if (paused)
        {
            _pauseDspTime = AudioSettings.dspTime;
            foreach (SoundEmitter emitter in _emitters)
            {
                emitter.PauseAll(_pauseDspTime);
            }

            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            _paused = true;
            Debug.Log($"[SoundsManager] - paused at {SequenceTime:0.00}s");
        }
        else
        {
            double shift = AudioSettings.dspTime - _pauseDspTime;
            _startDspTime += shift;
            foreach (SoundEmitter emitter in _emitters)
            {
                emitter.ResumeAll(shift);
            }

            Time.timeScale = _timeScaleBeforePause;
            _paused = false;
            Debug.Log($"[SoundsManager] - resumed at {SequenceTime:0.00}s");
        }
    }

    void OnDisable()
    {
        SetPaused(false);
    }

    private IEnumerator PlaySequence()
    {
        if (_waitForInputOnWeb && Application.platform == RuntimePlatform.WebGLPlayer)
        {
            bool pressed = false;
            InputSystem.onAnyButtonPress.CallOnce(control => pressed = true);
            while (!pressed)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(_startDelay);

        List<SoundCue> cues = new List<SoundCue>(_cues);
        cues.Sort((a, b) => a.Time.CompareTo(b.Time));
        List<SequenceEvent> events = new List<SequenceEvent>(_events);
        events.Sort((a, b) => a.Time.CompareTo(b.Time));

        _startDspTime = AudioSettings.dspTime + 0.2;
        _started = true;
        Debug.Log($"[SoundsManager] - sequence started with {cues.Count} cues and {events.Count} events");

        int cueIndex = 0;
        int eventIndex = 0;
        while (cueIndex < cues.Count || eventIndex < events.Count)
        {
            float time = SequenceTime;

            while (cueIndex < cues.Count && cues[cueIndex].Time <= time + _scheduleAhead)
            {
                Schedule(cues[cueIndex]);
                cueIndex++;
            }

            while (eventIndex < events.Count && events[eventIndex].Time <= time)
            {
                Trigger(events[eventIndex]);
                eventIndex++;
            }

            yield return null;
        }

        Finished = true;
        Debug.Log("[SoundsManager] - sequence finished");
    }

    private void Schedule(SoundCue cue)
    {
        if (cue.Clip == null)
        {
            return;
        }

        double dspTime = _startDspTime + cue.Time;
        if (cue.Channel == SoundChannel.Voice && VCManager != null)
        {
            VCManager.Play(cue, dspTime, _effects);
        }
        else if (SFXManager != null)
        {
            SFXManager.Play(cue, dspTime);
        }
    }

    private void Trigger(SequenceEvent sequenceEvent)
    {
        if (sequenceEvent.Type == SequenceEventType.End)
        {
            Finished = true;
            return;
        }

        if (_effects == null)
        {
            return;
        }

        switch (sequenceEvent.Type)
        {
            case SequenceEventType.Shake:
                _effects.Shake(sequenceEvent.Duration, sequenceEvent.Strength);
                break;
            case SequenceEventType.Flicker:
                _effects.Flicker(sequenceEvent.Time, sequenceEvent.Duration, sequenceEvent.Strength, sequenceEvent.Seed);
                break;
            case SequenceEventType.Blackout:
                _effects.Blackout(sequenceEvent.Duration);
                break;
        }
    }
}
