using System.Threading.Tasks;
using UnityEngine;

public class Photo : MonoBehaviour
{
    [SerializeField]
    private Camera _photoCamera;
    [SerializeField]
    private float _holdDistance;
    [SerializeField]
    private float _holdSize;
    [SerializeField]
    private float _moveTime;
    [SerializeField]
    private float _expandTime;
    [SerializeField]
    private float _glossFadeTime;
    public Camera PhotoCamera { get { return _photoCamera; } }

    public async Task FillView(Camera view)
    {
        transform.SetParent(null);
        _ = FadeGloss();
        _photoCamera.fieldOfView = view.fieldOfView;
        _photoCamera.aspect = view.aspect;

        float height = 2 * _holdDistance * Mathf.Tan(view.fieldOfView * 0.5f * Mathf.Deg2Rad);
        Vector3 viewSize = new Vector3(height * view.aspect, height, 1);
        Vector3 position = view.transform.position + view.transform.forward * _holdDistance;

        await MoveTo(position, view.transform.rotation, viewSize * _holdSize, _moveTime);
        await MoveTo(position, view.transform.rotation, viewSize, _expandTime);
    }

    private async Task FadeGloss()
    {
        Material material = GetComponent<Renderer>().material;
        Color specular = material.GetColor("_SpecColor");
        float smoothness = material.GetFloat("_Smoothness");
        float t = 0;
        while (t < 1)
        {
            t = Mathf.Min(t + Time.deltaTime / _glossFadeTime, 1);
            material.SetColor("_SpecColor", Color.Lerp(specular, Color.black, t));
            material.SetFloat("_Smoothness", Mathf.Lerp(smoothness, 0, t));
            await Awaitable.NextFrameAsync();
        }
    }

    private async Task MoveTo(Vector3 position, Quaternion rotation, Vector3 scale, float time)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 startScale = transform.localScale;
        float t = 0;
        while (t < 1)
        {
            t = Mathf.Min(t + Time.deltaTime / time, 1);
            float smooth = Mathf.SmoothStep(0, 1, t);
            transform.SetPositionAndRotation(Vector3.Lerp(startPosition, position, smooth), Quaternion.Slerp(startRotation, rotation, smooth));
            transform.localScale = Vector3.Lerp(startScale, scale, smooth);
            await Awaitable.NextFrameAsync();
        }
    }
}
