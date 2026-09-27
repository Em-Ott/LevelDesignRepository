using UnityEngine;
using UnityEngine.InputSystem;
using Unity​Engine.Input​System.Controls;

/*
Basic Player movement and camera scripts have been ported over and slightly modified from a prior project of Emily Ott's
which uses the same player movement and a third person following camera with mouse look. 
*/
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;
    [SerializeField]
    private float _minClampForYRotation;
    [SerializeField]
    private float _maxClampForYRotation;
    [SerializeField]
    private float _mouseSensitivity;
    [SerializeField]
    private Camera _playerCamera;
    private Rigidbody _rigidBody;
    private Keyboard _keyboard;
    private Mouse _mouse;
    private float yaw;

    void Awake()
    {
        // For larger projects the below line should be moved to an input manager class
        _keyboard = Keyboard.current;
        _mouse = Mouse.current;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        if (!_rigidBody) { Debug.Log("Please attach a rigidbody to player."); }
    }

    // Update is called once per frame
    // Recommendation: Move to a separate input manager class which calls PlayerMovement's HandleMovement if you have many 
    // things being checked in update (checks for keys being pressed/mouse movement). For smaller projects it is fine to be
    // kept here.
    void Update()
    {
        (float x, float z) movement = CheckForMovement();
        Quaternion rotation = MouseLook();

        HandleMouseLook(rotation);
        HandleMovement(movement.x, 0, movement.z);
    }

    // Adjusts the rotation of the body off the given quaternion
    public void HandleMouseLook(Quaternion rotation)
    {
        _rigidBody.rotation = rotation;
    }

    // Adjusts the rigid bodies velocity off the given linear values
    public void HandleMovement(float xMovement, float yMovement, float zMovement)
    {
        Transform playerTransform = _rigidBody.transform;
        Vector3 forwardMovement = playerTransform.forward * zMovement * _moveSpeed;
        Vector3 rightMovement = playerTransform.right * xMovement * _moveSpeed;
        Vector3 upMovement = playerTransform.up * yMovement * _moveSpeed;
        Vector3 movement = forwardMovement + rightMovement + upMovement;
        _rigidBody.linearVelocity = movement;
    }

    // Returns whether keys which would effect the x and z position have been pressed
    // If keys for both directions (right + left) are pressed no movement is made
    private (float, float) CheckForMovement()
    {
        float zMovement = 0;
        float xMovement = 0;

        if (_keyboard.wKey.isPressed) { zMovement += 1; }
        if (_keyboard.aKey.isPressed) { xMovement -= 1; }
        if (_keyboard.dKey.isPressed) { xMovement += 1; }
        if (_keyboard.sKey.isPressed) { zMovement -= 1; }

        return (xMovement, zMovement);
    }

    // Returns where the mouse is looking at
    private Quaternion MouseLook()
    {
        Vector2 mouseRotation = _mouse.delta.ReadValue();
        Transform cameraTransform = _playerCamera.gameObject.transform;

        yaw += mouseRotation.x * _mouseSensitivity;

        Quaternion playerRotation = Quaternion.Euler(0, yaw, 0);
        return playerRotation;
    }
}
