using UnityEngine;

public class DialogueInstance : MonoBehaviour
{
    [SerializeField]
    private int _readOrder;
    public int ReadOrder { get { return _readOrder; } }
    [System.NonSerialized]
    public bool Read;
    [SerializeField]
    private DialogueObject _dialogue;
    public DialogueObject Dialogue { get { return _dialogue; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Read = false;
    }
}
