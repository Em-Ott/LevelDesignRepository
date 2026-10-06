using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueView : MonoBehaviour
{
    [SerializeField]
    private GameObject _dialogueUI;
    [SerializeField]
    private GameObject _choiceUI;
    [SerializeField]
    private DialogueText _text;
    [SerializeField]
    private Button[] _choiceButton;
    private DialogueOption _choiceA;
    private DialogueOption _choiceB;
    private InputAction _mouseClickAction;
    private bool _playerDoneWithDialogue;

    void Start()
    {
        _mouseClickAction = InputSystem.actions.FindAction("Attack");
        _mouseClickAction.performed += NextDialogue;
        Debug.Log(_choiceUI);
        _choiceUI.SetActive(false);
        _dialogueUI.SetActive(false);
        //_choiceButton[0].onClick += FirstChoiceButtonClicked;
        //_choiceButton[1].onClick += SecondChoiceButtonClicked;
    }

    public async Task OnStartDialogue(DialogueObject dialogue)
    {
        Debug.Log("starting dialogue!");
        _playerDoneWithDialogue = false;
        _dialogueUI.SetActive(true);
        _choiceUI.SetActive(false);
        await RunDialogue(dialogue);
        Debug.Log("dialogue done :9");
        _dialogueUI.SetActive(false);
    }

    private void NextDialogue(InputAction.CallbackContext clickInformation)
    {
        _playerDoneWithDialogue = true;
    }

    private void FirstChoiceButtonClicked()
    {

    }

    private void SecondChoiceButtonClicked()
    {

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
        else
        {
            _dialogueUI.SetActive(false);
        }
    }

    private void DisplayChoices(DialogueOption[] options)
    {
        _choiceUI.SetActive(true);
        int i = 0;
        foreach (DialogueOption option in options)
        {
            //_text.SetCharacterText(option.Choice.ChoiceText, i);
            i++;
        }
    }
}
