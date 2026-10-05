using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueView : MonoBehaviour
{
    [SerializeField]
    private GameObject _dialogueUI;
    [SerializeField]
    private GameObject _choiceUI;
    [SerializeField]
    private DialogueText _text;
    private InputAction _mouseClickAction;
    private bool _playerDoneWithDialogue;

    void Start()
    {
        _mouseClickAction = InputSystem.actions.FindAction("Attack");
        _mouseClickAction.performed += NextDialogue;
    }

    public async Task OnStartDialogue(DialogueObject dialogue)
    {
        _playerDoneWithDialogue = false;
        _dialogueUI.SetActive(true);
        _choiceUI.SetActive(false);
        await RunDialogue(dialogue);
        _dialogueUI.SetActive(false);
    }

    private void NextDialogue(InputAction.CallbackContext clickInformation)
    {
        _playerDoneWithDialogue = true;
    }

    private async Task RunDialogue(DialogueObject dialogue)
    {
        for (int i = 0; i < dialogue.ConcurrentDialogue.Length; i++)
        {
            _playerDoneWithDialogue = false;
            _text.SetCharacterText(dialogue.CharacterName);
            _text.SetSpeakingText(dialogue.ConcurrentDialogue[i]);

            while (!_playerDoneWithDialogue)
            {
                await Awaitable.WaitForSecondsAsync(0.1f);
            }
        }

        if (dialogue.Choices.Length > 0)
        {
            DisplayChoices(dialogue.Choices);
        }
    }

    private void DisplayChoices(DialogueOption[] options)
    {
        _choiceUI.SetActive(true);
    }
}
