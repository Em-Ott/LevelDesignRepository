using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private Vector3 _interactRange;
    [SerializeField]
    private DialogueView _view;
    [SerializeField]
    private PlayerMove _move;
    [SerializeField]
    private PlayerLookFPov _look;
    [SerializeField]
    private DialogueObject _startingDialogue;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private float _photoRange;
    [SerializeField]
    private float _photoHoldTime;
    private InputAction _interactAction;
    private bool _inDialogue;
    private bool _inPhoto;
    private float _holdTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _interactAction = InputSystem.actions.FindAction("Interact");
        _inDialogue = false;
        _interactAction.performed += HandleInteract;

        if (_startingDialogue != null)
        {
            _ = HandleFlagChange(_startingDialogue);
        }
    }

    void Update()
    {
        if (!_inDialogue && !_inPhoto && _interactAction.IsPressed()
            && Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, _photoRange)
            && hit.collider.TryGetComponent(out Photo photo))
        {
            _holdTimer += Time.deltaTime;
            if (_holdTimer >= _photoHoldTime)
            {
                _ = EnterPhoto(photo);
            }
        }
        else
        {
            _holdTimer = 0;
        }
    }

    void HandleInteract(InputAction.CallbackContext lookInformation)
    {
        if (_inDialogue) { return; }

        Collider[] objectsNearby = Physics.OverlapBox(this.gameObject.transform.position, _interactRange);
        List<DialogueInstance> allDialogueObjects = new List<DialogueInstance>();
        foreach (Collider coll in objectsNearby)
        {
            DialogueInstance[] possibleDialogueOptions = coll.gameObject.GetComponentsInChildren<DialogueInstance>();
            allDialogueObjects.AddRange(possibleDialogueOptions);
        }

        DialogueInstance highestPriorityObject = null;
        int priority = 100;
        for (int i = 0; i < allDialogueObjects.Count; i++)
        {
            if (!allDialogueObjects[i].Read && allDialogueObjects[i].ReadOrder < priority)
            {
                highestPriorityObject = allDialogueObjects[i];
                priority = allDialogueObjects[i].ReadOrder;
            }
        }

        if (!highestPriorityObject)
        {
            return;
        }
        else
        {
            _ = HandleFlagChange(highestPriorityObject.Dialogue);
            highestPriorityObject.Read = true;
        }
    }

    async Task HandleFlagChange(DialogueObject currentObject)
    {
        _inDialogue = true;
        _move.AllowMove(false);
        _look.AllowLook(false);
        await _view.OnStartDialogue(currentObject);
        _move.AllowMove(true);
        _look.AllowLook(true);
        _inDialogue = false;
    }

    async Task EnterPhoto(Photo photo)
    {
        _inPhoto = true;
        _move.AllowMove(false);
        _look.AllowLook(false);
        await photo.FillView(_camera);
        _look.SetLook(photo.PhotoCamera.transform.rotation);
        transform.position += photo.PhotoCamera.transform.position - _camera.transform.position;
        Destroy(photo.gameObject);
        _move.AllowMove(true);
        _look.AllowLook(true);
        _inPhoto = false;
    }
}
