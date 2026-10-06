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
    private InputAction _interactAction;
    private bool _inDialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _interactAction = InputSystem.actions.FindAction("Interact");
        Debug.Log(_interactAction);
        Debug.Log(_interactAction.enabled);
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

        Debug.Log(allDialogueObjects.Count);
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
            Debug.Log("trying to start");
            _ = HandleFlagChange(highestPriorityObject);
            highestPriorityObject.Read = true;
        }
    }

    async Task HandleFlagChange(DialogueInstance currentInstance)
    {
        _inDialogue = true;
        Debug.Log("trying...");
        await _view.OnStartDialogue(currentInstance.Dialogue);
        Debug.Log("done");
        _inDialogue = false;
    }
}
