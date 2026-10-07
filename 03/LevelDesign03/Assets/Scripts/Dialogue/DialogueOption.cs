using UnityEngine;

[CreateAssetMenu(fileName = "DialogueOption", menuName = "Scriptable Objects/DialogueOption")]
public class DialogueOption : ScriptableObject
{
    [SerializeField]
    private ChoiceDialogueStruct[] _choice;
    public ChoiceDialogueStruct[] Choices { get { return _choice; } }
}

[System.Serializable]
public struct ChoiceDialogueStruct
{
    public string ChoiceText;
    public bool Correct;
    public bool ChoiceKillsYou;
    public bool SecondPhaseStart;
    public DialogueObject Result;
}

