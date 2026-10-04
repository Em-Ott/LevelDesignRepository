using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookFPov : MonoBehaviour
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
    [SerializeField]
    private Transform _playerTransform;
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
            (float x, float y) rotation = MouseLook(_mouseLookAction.ReadValue<Vector2>());
            HandleMouseLook(rotation.x, rotation.y);
        }
    }

    // Adjusts the rotation of the body off the given quaternion
    public void HandleMouseLook(float pitch, float yaw)
    {
        Quaternion playerRotation = Quaternion.Euler(0, yaw, 0);
        Quaternion cameraRotation = Quaternion.Euler(pitch, 0, 0);

        _playerTransform.rotation = playerRotation;
        _cameraTransform.localRotation = cameraRotation;
    }

    // Returns where the mouse is looking at
    private (float x, float y) MouseLook(Vector2 mouseInput)
    {
        Transform cameraTransform = _playerCamera.gameObject.transform;

        yaw += mouseInput.x * _mouseSensitivity;
        pitch -= mouseInput.y * _mouseSensitivity;

        if (_minClampForYRotation != _maxClampForYRotation) { yaw = Mathf.Clamp(yaw, _minClampForYRotation, _maxClampForYRotation); }
        if (_minClampForXRotation != _maxClampForXRotation) { pitch = Mathf.Clamp(pitch, _minClampForXRotation, _maxClampForXRotation); }

        return (pitch, yaw);
    }
}
