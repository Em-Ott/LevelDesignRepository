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
    private InputAction _interactAction;
    private bool _inDialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _interactAction = InputSystem.actions.FindAction("Interact");
        _inDialogue = false;
        _interactAction.performed += HandleInteract;
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
            _ = HandleFlagChange(highestPriorityObject);
            highestPriorityObject.Read = true;
        }
    }

    async Task HandleFlagChange(DialogueInstance currentInstance)
    {
        _inDialogue = true;
        _move.AllowMove(false);
        _look.AllowLook(false);
        await _view.OnStartDialogue(currentInstance.Dialogue);
        _move.AllowMove(true);
        _look.AllowLook(true);
        _inDialogue = false;
    }
}
