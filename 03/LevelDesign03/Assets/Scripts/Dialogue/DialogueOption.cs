using UnityEngine;

[CreateAssetMenu(fileName = "DialogueOption", menuName = "Scriptable Objects/DialogueOption")]
public class DialogueOption : ScriptableObject
{
    [SerializeField]
    private ChoiceDialogueStruct[] _choice;
    public ChoiceDialogueStruct[] Choices { get { return _choice; } }
}

public struct ChoiceDialogueStruct
{
    public string ChoiceText;
    public bool Correct;
    public DialogueObject Result;
}

