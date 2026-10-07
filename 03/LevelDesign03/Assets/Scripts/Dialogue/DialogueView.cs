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
    private Button[] _choiceButtons;
    [SerializeField]
    private GameObject _lossScreen;
    [SerializeField]
    private GameObject _happyGF;
    [SerializeField]
    private GameObject _scaryGF;
    private InputAction _mouseClickAction;
    private bool _playerDoneWithDialogue;
    private int _playerMadeChoice;

    void Start()
    {
        _mouseClickAction = InputSystem.actions.FindAction("Attack");
        _mouseClickAction.performed += NextDialogue;
        _choiceUI.SetActive(false);
        _dialogueUI.SetActive(false);

        for (int i = 0; i < _choiceButtons.Length; i++)
        {
            int index = i;
            _choiceButtons[i].onClick.AddListener(() => ChoiceButtonClicked(index));
        }
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

    private void ChoiceButtonClicked(int index)
    {
        _playerMadeChoice = index;
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

        if (dialogue.Choices != null)
        {
            await DisplayChoices(dialogue.Choices);
        }
        else if (dialogue.NextDialogue != null)
        {
            await OnStartDialogue(dialogue.NextDialogue);
        }
        else
        {
            _dialogueUI.SetActive(false);
        }
    }

    private async Task DisplayChoices(DialogueOption options)
    {
        _playerMadeChoice = -1;
        _choiceUI.SetActive(true);

        // Update dialogue buttons text
        for (int i = 0; i < options.Choices.Length; i++)
        {
            _text.SetChoiceBoxText(options.Choices[i].ChoiceText, i);
        }

        // Wait for player to make choice (click)
        while (_playerMadeChoice == -1)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
        }


        ChoiceDialogueStruct choice = options.Choices[_playerMadeChoice];
        _choiceUI.SetActive(false);

        if (choice.SecondPhaseStart)
        {
            _scaryGF.SetActive(true);
            _happyGF.SetActive(false);
        }

        if (choice.ChoiceKillsYou)
        {
            if (choice.Result != null)
            {
                await this.OnStartDialogue(choice.Result);
            }
            _lossScreen.SetActive(true);
            return;
        }

        if (choice.Correct)
        {

        }
        else
        {

        }

        // Continue dialogue if appropriate
        if (choice.Result != null)
        {
            _playerMadeChoice = -1;
            await this.OnStartDialogue(choice.Result);
        }
    }
}
