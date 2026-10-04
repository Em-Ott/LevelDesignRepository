using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    private float _minClampForYRotation;
    [SerializeField]
    private float _maxClampForYRotation;
    [SerializeField]
    private float _minClampForXRotation;
    [SerializeField]
    private float _maxClampForXRotation;
    [SerializeField]
    private float _mouseSensitivity;
    [SerializeField]
    private Camera _playerCamera;
    private Transform _cameraTransform;
    private InputAction _mouseLookAction;
    private float yaw;
    private float pitch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraTransform = _playerCamera.gameObject.transform;
        if (!_cameraTransform) { Debug.Log("Camera is missing a transform."); }
        // See Edit -> Project Settings -> Input Systems Package for actions
        _mouseLookAction = InputSystem.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        if (_mouseLookAction.WasPerformedThisFrame())
        {
            Quaternion rotation = MouseLook(_mouseLookAction.ReadValue<Vector2>());
            HandleMouseLook(rotation);
        }
    }

    // Adjusts the rotation of the body off the given quaternion
    public void HandleMouseLook(Quaternion rotation)
    {
        _cameraTransform.localRotation = rotation;
    }

    // Returns where the mouse is looking at
    private Quaternion MouseLook(Vector2 mouseInput)
    {
        Transform cameraTransform = _playerCamera.gameObject.transform;

        yaw += mouseInput.x * _mouseSensitivity;
        pitch -= mouseInput.y * _mouseSensitivity;

        yaw = Mathf.Clamp(yaw, _minClampForYRotation, _maxClampForYRotation);
        pitch = Mathf.Clamp(pitch, _minClampForXRotation, _maxClampForXRotation);

        Quaternion playerRotation = Quaternion.Euler(pitch, yaw, 0);
        return playerRotation;
    }
}
