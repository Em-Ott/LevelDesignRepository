using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    [SerializeField]
    private float _moveSpeed;
    private Rigidbody _rigidBody;
    private InputAction _movementAction;
    private Transform _playerTransform;
    private Vector2 _movementInput = Vector2.zero;
    private bool _allowMove = true;

    void Awake()
    {
        _movementAction = InputSystem.actions.FindAction("Move");
        _movementAction.performed += HandleMovement;
        _movementAction.canceled += HandleMovement;
        _rigidBody = GetComponentInChildren<Rigidbody>();
        if (!_rigidBody) { Debug.Log("Please attach a rigidbody to player."); }
        _playerTransform = _rigidBody.transform;
    }

    void FixedUpdate()
    {
        if (!_allowMove) { return; }
        Vector3 forwardMovement = _playerTransform.forward * _movementInput.y * _moveSpeed;
        Vector3 rightMovement = _playerTransform.right * _movementInput.x * _moveSpeed;
        _rigidBody.linearVelocity = forwardMovement + rightMovement;
    }

    public void AllowMove(bool allowed)
    {
        _allowMove = allowed;
        if (!allowed)
        {
            _rigidBody.linearVelocity = Vector3.zero;
        }
    }

    // Stores the latest movement input, velocity is applied relative to playerTransform in FixedUpdate
    public void HandleMovement(InputAction.CallbackContext movementInformation)
    {
        if (movementInformation.performed)
        {
            _movementInput = movementInformation.ReadValue<Vector2>();
        }
        else if (movementInformation.canceled)
        {
            _movementInput = Vector2.zero;
        }
    }
}
