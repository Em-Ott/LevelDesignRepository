using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SequenceEffects : MonoBehaviour
{
    private struct ShakeInstance
    {
        public float Start;
        public float Duration;
        public float Strength;
    }

    [Header("Camera Shake")]
    [SerializeField] private Transform _shakeTarget;
    [SerializeField] private float _shakeOffset = 0.035f;
    [SerializeField] private float _shakeAngle = 1.5f;
    [SerializeField] private float _shakeFrequency = 18f;

    [Header("Lights")]
    [SerializeField] private List<Light> _lights = new List<Light>();
    [SerializeField, Range(0f, 1f)] private float _flickerDarkness = 0.75f;

    [Header("Baked Lightmaps")]
    [Tooltip("Lightmap-N_comp_light textures from the lights-off bake, in index order. The scene's own lightmaps are the lights-on set.")]
    [SerializeField] private Texture2D[] _offLightmaps = new Texture2D[0];
    [Tooltip("Lightmap-N_comp_dir textures from the lights-off bake. Leave empty for Non-Directional lightmaps.")]
    [SerializeField] private Texture2D[] _offDirectionalLightmaps = new Texture2D[0];
    [Tooltip("Light levels below this use the lights-off lightmaps.")]
    [SerializeField, Range(0f, 1f)] private float _lightmapThreshold = 0.2f;
    [Tooltip("Renderers whose emission follows the light level (the ceiling light glass).")]
    [SerializeField] private List<Renderer> _emissiveRenderers = new List<Renderer>();

    [Header("Screen")]
    [SerializeField] private Image _fade;
    [SerializeField] private TextMeshProUGUI _subtitle;
    [SerializeField, Range(0f, 0.5f)] private float _subtitleOutline = 0.2f;

    private readonly List<ShakeInstance> _shakes = new List<ShakeInstance>();
    private SoundsManager _manager;
    private Vector3 _restPosition;
    private Quaternion _lastRotation;
    private Quaternion _lastOffset = Quaternion.identity;
    private float[] _lightIntensity = new float[0];
    private LightmapData[] _onLightmapData;
    private LightmapData[] _offLightmapData;
    private bool _lightmapsOn = true;
    private Color[][] _emissionColors = new Color[0][];
    private MaterialPropertyBlock _emissionBlock;
    private Coroutine _flicker;
    private Coroutine _subtitleRoutine;
    private bool _blackedOut;
    private float _noiseSeed;

    private float Clock => _manager != null && _manager.Started ? _manager.SequenceTime : Time.time;

    void Start()
    {
        _manager = GetComponentInParent<SoundsManager>();

        if (_shakeTarget == null && Camera.main != null)
        {
            _shakeTarget = Camera.main.transform;
        }

        if (_shakeTarget != null)
        {
            _restPosition = _shakeTarget.localPosition;
            _lastRotation = _shakeTarget.localRotation;
        }

        if (_lights.Count == 0)
        {
            _lights.AddRange(FindObjectsByType<Light>(FindObjectsSortMode.None));
        }

        _lightIntensity = new float[_lights.Count];
        for (int i = 0; i < _lights.Count; i++)
        {
            _lightIntensity[i] = _lights[i] != null ? _lights[i].intensity : 0f;
        }

        SetupLightmaps();
        SetupEmission();

        SetFade(0f);
        if (_subtitle != null)
        {
            _subtitle.text = string.Empty;
            StyleSubtitle();
        }

        _noiseSeed = Random.Range(0f, 100f);
    }

    public void Shake(float duration, float strength)
    {
        _shakes.Add(new ShakeInstance { Start = Time.time, Duration = Mathf.Max(0.05f, duration), Strength = strength });
    }

    public void Flicker(float startTime, float duration, float strength, int seed)
    {
        if (_flicker != null)
        {
            StopCoroutine(_flicker);
        }

        _flicker = StartCoroutine(FlickerRoutine(startTime, duration, strength, seed));
    }

    public void Blackout(float duration)
    {
        _blackedOut = true;
        if (_flicker != null)
        {
            StopCoroutine(_flicker);
            _flicker = null;
        }

        StartCoroutine(BlackoutRoutine(duration));
    }

    public void ShowSubtitle(string text, Color color, float duration)
    {
        if (_subtitle == null || _blackedOut)
        {
            return;
        }

        if (_subtitleRoutine != null)
        {
            StopCoroutine(_subtitleRoutine);
        }

        _subtitleRoutine = StartCoroutine(SubtitleRoutine(text, color, duration));
    }

    public static float FlickerLevel(int seed, float time, float strength)
    {
        uint slot = (uint)Mathf.Max(0, Mathf.FloorToInt(time * 16f));
        uint h = unchecked(slot * 2654435761u + (uint)seed * 40503u + 12345u);
        h ^= h >> 13;
        h = unchecked(h * 1274126177u);
        h ^= h >> 16;
        float r = (h & 0xFFFF) / 65535f;
        float off = 0.18f + 0.32f * strength;

        if (r < off)
        {
            return 0.04f;
        }

        if (r < off + 0.22f)
        {
            return 0.38f;
        }

        return 1f;
    }

    void LateUpdate()
    {
        if (_shakeTarget == null)
        {
            return;
        }

        float now = Time.time;
        float amount = 0f;
        for (int i = _shakes.Count - 1; i >= 0; i--)
        {
            ShakeInstance shake = _shakes[i];
            float progress = (now - shake.Start) / shake.Duration;
            if (progress >= 1f)
            {
                _shakes.RemoveAt(i);
                continue;
            }

            float attack = Mathf.Clamp01((now - shake.Start) / 0.04f);
            float decay = shake.Duration >= 1.5f ? 1f - progress * progress * progress : (1f - progress) * (1f - progress);
            amount += shake.Strength * attack * decay;
        }

        amount = Mathf.Min(amount, 1.5f);

        Quaternion current = _shakeTarget.localRotation;
        Quaternion baseRotation = current == _lastRotation ? current * Quaternion.Inverse(_lastOffset) : current;
        Quaternion offset = Quaternion.identity;
        Vector3 position = _restPosition;

        if (amount > 0.0001f)
        {
            float t = now * _shakeFrequency;
            position += new Vector3(Noise(t, 0f), Noise(t, 17f), Noise(t, 31f)) * (_shakeOffset * amount);
            offset = Quaternion.Euler(
                Noise(t, 47f) * _shakeAngle * amount,
                Noise(t, 59f) * _shakeAngle * amount * 0.6f,
                Noise(t, 71f) * _shakeAngle * amount * 0.8f);
        }

        _shakeTarget.localPosition = position;
        _shakeTarget.localRotation = baseRotation * offset;
        _lastRotation = _shakeTarget.localRotation;
        _lastOffset = offset;
    }

    private float Noise(float time, float lane)
    {
        return Mathf.PerlinNoise(time, lane + _noiseSeed) * 2f - 1f;
    }

    private void StyleSubtitle()
    {
        Material material = _subtitle.fontMaterial;
        if (_subtitleOutline > 0f)
        {
            material.EnableKeyword("OUTLINE_ON");
            material.SetFloat("_OutlineWidth", _subtitleOutline);
            material.SetColor("_OutlineColor", new Color(0f, 0f, 0f, 0.9f));
        }

        material.EnableKeyword("UNDERLAY_ON");
        material.SetColor("_UnderlayColor", new Color(0f, 0f, 0f, 0.75f));
        material.SetFloat("_UnderlayOffsetX", 0.5f);
        material.SetFloat("_UnderlayOffsetY", -0.5f);
        material.SetFloat("_UnderlayDilate", 0.4f);
        material.SetFloat("_UnderlaySoftness", 0.5f);
        _subtitle.UpdateMeshPadding();
    }

    private IEnumerator FlickerRoutine(float startTime, float duration, float strength, int seed)
    {
        while (Clock - startTime < duration)
        {
            ApplyLight(FlickerLevel(seed, Clock - startTime, strength));
            yield return null;
        }

        ApplyLight(1f);
        _flicker = null;
    }

    private IEnumerator BlackoutRoutine(float duration)
    {
        if (_subtitle != null)
        {
            _subtitle.text = string.Empty;
        }

        float start = Time.time;
        float from = _fade != null ? _fade.color.a : 0f;
        while (duration > 0f && Time.time - start < duration)
        {
            float t = (Time.time - start) / duration;
            SetFade(Mathf.Lerp(from, 1f, t));
            SetLights(1f - t);
            yield return null;
        }

        SetFade(1f);
        SetLights(0f);
    }

    private IEnumerator SubtitleRoutine(string text, Color color, float duration)
    {
        _subtitle.text = text;
        _subtitle.color = color;
        yield return new WaitForSeconds(duration);
        _subtitle.text = string.Empty;
        _subtitleRoutine = null;
    }

    private void ApplyLight(float level)
    {
        if (_blackedOut)
        {
            return;
        }

        SetLights(level);
        SetFade((1f - level) * _flickerDarkness);
    }

    private void SetLights(float level)
    {
        for (int i = 0; i < _lights.Count; i++)
        {
            if (_lights[i] != null)
            {
                _lights[i].intensity = _lightIntensity[i] * level;
            }
        }

        SetLightmaps(level >= _lightmapThreshold);
        SetEmission(level);
    }

    private void SetupLightmaps()
    {
        _onLightmapData = LightmapSettings.lightmaps;
        _offLightmapData = null;
        _lightmapsOn = true;

        if (_offLightmaps == null || _offLightmaps.Length == 0)
        {
            return;
        }

        if (_onLightmapData == null || _onLightmapData.Length != _offLightmaps.Length)
        {
            Debug.LogWarning($"SequenceEffects: {_offLightmaps.Length} lights-off lightmaps assigned, but the scene has {(_onLightmapData == null ? 0 : _onLightmapData.Length)}. Lightmap flicker disabled.", this);
            return;
        }

        _offLightmapData = new LightmapData[_offLightmaps.Length];
        for (int i = 0; i < _offLightmaps.Length; i++)
        {
            _offLightmapData[i] = new LightmapData
            {
                lightmapColor = _offLightmaps[i],
                lightmapDir = _offDirectionalLightmaps != null && i < _offDirectionalLightmaps.Length ? _offDirectionalLightmaps[i] : null,
                shadowMask = _onLightmapData[i].shadowMask
            };
        }
    }

    private void SetLightmaps(bool on)
    {
        if (_offLightmapData == null || on == _lightmapsOn)
        {
            return;
        }

        LightmapSettings.lightmaps = on ? _onLightmapData : _offLightmapData;
        _lightmapsOn = on;
    }

    private void SetupEmission()
    {
        _emissionBlock = new MaterialPropertyBlock();
        _emissionColors = new Color[_emissiveRenderers.Count][];
        for (int i = 0; i < _emissiveRenderers.Count; i++)
        {
            Renderer emissive = _emissiveRenderers[i];
            Material[] materials = emissive != null ? emissive.sharedMaterials : new Material[0];
            _emissionColors[i] = new Color[materials.Length];
            for (int m = 0; m < materials.Length; m++)
            {
                bool emits = materials[m] != null && materials[m].HasProperty("_EmissionColor") && materials[m].IsKeywordEnabled("_EMISSION");
                _emissionColors[i][m] = emits ? materials[m].GetColor("_EmissionColor") : Color.clear;
            }
        }
    }

    private void SetEmission(float level)
    {
        for (int i = 0; i < _emissiveRenderers.Count; i++)
        {
            Renderer emissive = _emissiveRenderers[i];
            if (emissive == null)
            {
                continue;
            }

            for (int m = 0; m < _emissionColors[i].Length; m++)
            {
                if (_emissionColors[i][m] == Color.clear)
                {
                    continue;
                }

                emissive.GetPropertyBlock(_emissionBlock, m);
                _emissionBlock.SetColor("_EmissionColor", _emissionColors[i][m] * level);
                emissive.SetPropertyBlock(_emissionBlock, m);
            }
        }
    }

    void OnDestroy()
    {
        if (_offLightmapData != null && !_lightmapsOn)
        {
            LightmapSettings.lightmaps = _onLightmapData;
        }
    }

    private void SetFade(float alpha)
    {
        if (_fade == null)
        {
            return;
        }

        _fade.color = new Color(0f, 0f, 0f, alpha);
        _fade.enabled = alpha > 0.001f;
    }
}
