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

    [Header("Next Dialogue to Different Character or Choices")]
    [Tooltip("PLEASE ONLY DO ONE. Defaults to choices if both.")]
    [SerializeField]
    private DialogueObject _nextDialogue;
    public DialogueObject NextDialogue { get { return _nextDialogue; } }
    [SerializeField]
    private DialogueOption _choices;
    public DialogueOption Choices { get { return _choices; } }
}
