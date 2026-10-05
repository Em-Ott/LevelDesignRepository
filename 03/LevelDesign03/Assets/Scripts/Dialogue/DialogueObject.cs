using UnityEngine;

[CreateAssetMenu(fileName = "DialogueObject", menuName = "Scriptable Objects/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField]
    private string[] _concurrentDialogue;
    public string[] ConcurrentDialogue { get { return _concurrentDialogue; } }
    [SerializeField]
    private string _characterName;
    public string CharacterName { get { return _characterName; } }

    [SerializeField]
    private DialogueOption[] _choices;
    public DialogueOption[] Choices { get { return _choices; } }

}
