using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EmitterKey
{
    public float Time;
    public Vector3 Position;
}

public class SoundEmitter : MonoBehaviour
{
    [SerializeField] private bool _spatial = true;
    [SerializeField, Range(0f, 360f)] private float _spread = 40f;
    [SerializeField] private float _minDistance = 60f;
    [SerializeField] private float _maxDistance = 120f;
    [SerializeField] private List<EmitterKey> _path = new List<EmitterKey>();

    private readonly List<AudioSource> _sources = new List<AudioSource>();
    private readonly List<double> _busyUntil = new List<double>();
    private readonly List<double> _startAt = new List<double>();
    private readonly List<int> _pauseState = new List<int>();

    private const int NotPaused = 0;
    private const int PausedPlaying = 1;
    private const int PausedPending = 2;

    public bool Spatial
    {
        get { return _spatial; }
        set { _spatial = value; }
    }

    public void Follow(float time)
    {
        if (_path.Count == 0)
        {
            return;
        }

        transform.localPosition = Evaluate(time);
    }

    public Vector3 Evaluate(float time)
    {
        if (time <= _path[0].Time)
        {
            return _path[0].Position;
        }

        for (int i = 0; i < _path.Count - 1; i++)
        {
            EmitterKey from = _path[i];
            EmitterKey to = _path[i + 1];
            if (time <= to.Time)
            {
                float t = to.Time > from.Time ? Mathf.SmoothStep(0f, 1f, (time - from.Time) / (to.Time - from.Time)) : 1f;
                return Vector3.Lerp(from.Position, to.Position, t);
            }
        }

        return _path[_path.Count - 1].Position;
    }

    public AudioSource Schedule(AudioClip clip, float volume, double dspTime, AudioSource template)
    {
        int index = Acquire(dspTime + clip.length + 0.1, template);
        AudioSource source = _sources[index];
        _startAt[index] = dspTime;
        source.clip = clip;
        source.volume = volume;
        source.PlayScheduled(dspTime);
        return source;
    }

    public void PauseAll(double now)
    {
        for (int i = 0; i < _sources.Count; i++)
        {
            _pauseState[i] = NotPaused;
            if (_busyUntil[i] <= now)
            {
                continue;
            }

            if (_startAt[i] > now)
            {
                _sources[i].Stop();
                _pauseState[i] = PausedPending;
            }
            else if (_sources[i].isPlaying)
            {
                _sources[i].Pause();
                _pauseState[i] = PausedPlaying;
            }
        }
    }

    public void ResumeAll(double shift)
    {
        for (int i = 0; i < _sources.Count; i++)
        {
            if (_pauseState[i] == NotPaused)
            {
                continue;
            }

            _startAt[i] += shift;
            _busyUntil[i] += shift;
            if (_pauseState[i] == PausedPending)
            {
                _sources[i].PlayScheduled(_startAt[i]);
            }
            else
            {
                _sources[i].UnPause();
            }

            _pauseState[i] = NotPaused;
        }
    }

    private int Acquire(double busyUntil, AudioSource template)
    {
        double now = AudioSettings.dspTime;
        for (int i = 0; i < _sources.Count; i++)
        {
            if (_busyUntil[i] < now && !_sources[i].isPlaying)
            {
                _busyUntil[i] = busyUntil;
                Configure(_sources[i], template);
                return i;
            }
        }

        AudioSource source = gameObject.AddComponent<AudioSource>();
        Configure(source, template);
        _sources.Add(source);
        _busyUntil.Add(busyUntil);
        _startAt.Add(0);
        _pauseState.Add(NotPaused);
        return _sources.Count - 1;
    }

    private void Configure(AudioSource source, AudioSource template)
    {
        source.playOnAwake = false;
        source.loop = false;
        source.dopplerLevel = 0f;
        source.reverbZoneMix = 0f;
        source.spatialBlend = _spatial ? 1f : 0f;
        source.spread = _spatial ? _spread : 0f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = _minDistance;
        source.maxDistance = _maxDistance;

        if (template != null)
        {
            source.outputAudioMixerGroup = template.outputAudioMixerGroup;
            source.priority = template.priority;
            source.bypassEffects = template.bypassEffects;
            source.bypassListenerEffects = template.bypassListenerEffects;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _spatial ? new Color(1f, 0.55f, 0.15f, 0.9f) : new Color(0.4f, 0.7f, 1f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, 0.12f);

        if (_path.Count < 2)
        {
            return;
        }

        Transform parent = transform.parent;
        for (int i = 0; i < _path.Count - 1; i++)
        {
            Vector3 a = parent != null ? parent.TransformPoint(_path[i].Position) : _path[i].Position;
            Vector3 b = parent != null ? parent.TransformPoint(_path[i + 1].Position) : _path[i + 1].Position;
            Gizmos.DrawLine(a, b);
        }
    }
}
