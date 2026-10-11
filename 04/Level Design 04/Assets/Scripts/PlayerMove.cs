using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    [SerializeField]
    private float _moveSpeed;
    private Rigidbody _rigidBody;
    private InputAction _movementAction;
    private InputAction _mouseLookAction;
    private Transform _playerTransform;
    private Quaternion _previousPlayerRotation;
    private Vector3 _currentVelocity = Vector3.zero;
    private bool _allowMove = true;

    void Awake()
    {
        _movementAction = InputSystem.actions.FindAction("Move");
        _mouseLookAction = InputSystem.actions.FindAction("Look");
        _movementAction.performed += HandleMovement;
        _mouseLookAction.performed += HandleCameraMove;
        _movementAction.canceled += HandleMovement;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponentInChildren<Rigidbody>();
        if (!_rigidBody) { Debug.Log("Please attach a rigidbody to player."); }
        _playerTransform = _rigidBody.transform;
    }

    void FixedUpdate()
    {
        if (!_allowMove) { return; }
        _rigidBody.linearVelocity = _currentVelocity;
    }

    public void AllowMove(bool allowed)
    {
        _allowMove = allowed;
        if (!allowed)
        {
            _rigidBody.linearVelocity = Vector3.zero;
        }
    }

    // Adjust velocity to match new playerTransform.forward, this may need to be adjusted depending on FPov, ThirdPoV, etc.
    public void HandleCameraMove(InputAction.CallbackContext lookInformation)
    {
        if (_currentVelocity == Vector3.zero) { return; }

        Quaternion rotationChange = _playerTransform.rotation * Quaternion.Inverse(_previousPlayerRotation);
        _currentVelocity = rotationChange * _currentVelocity;
        _previousPlayerRotation = _playerTransform.rotation;
    }

    // Adjusts the rigid bodies velocity off the given linear values
    public void HandleMovement(InputAction.CallbackContext movementInformation)
    {
        if (movementInformation.performed)
        {
            Vector2 movementVector = movementInformation.ReadValue<Vector2>();
            float xMovement = movementVector.x;
            float zMovement = movementVector.y;

            Vector3 forwardMovement = _playerTransform.forward * zMovement * _moveSpeed;
            Vector3 rightMovement = _playerTransform.right * xMovement * _moveSpeed;
            Vector3 movement = forwardMovement + rightMovement;

            _currentVelocity = movement;
        }
        else if (movementInformation.canceled)
        {
            _currentVelocity = Vector3.zero;
        }
    }
}
