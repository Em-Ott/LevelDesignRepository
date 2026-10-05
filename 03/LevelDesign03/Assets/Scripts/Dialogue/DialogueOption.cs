using UnityEngine;

[CreateAssetMenu(fileName = "DialogueOption", menuName = "Scriptable Objects/DialogueOption")]
public class DialogueOption : ScriptableObject
{
    [SerializeField]
    private ChoiceDialogueStruct[] _choice;
}

public struct ChoiceDialogueStruct
{
    public string Choice;
    public bool Correct;
    public DialogueObject Result;
}

