using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GhostInteraction : MonoBehaviour
{
    [SerializeField]
    MeshRenderer decadeKnife;

    [SerializeField]
    GameObject ghostTextBox;

    [SerializeField]
    string ghostDialogue = "";

    private Keyboard keyboard;
    private MeshRenderer ghost;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyboard = Keyboard.current;
        ghost = gameObject.GetComponent<MeshRenderer>();
        ghost.enabled = false;
        ghostTextBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!decadeKnife.enabled) {
            ghost.enabled = true;
        } else {
            if (ghost.enabled == true) {
                ghostTextBox.SetActive(false);
            }
            ghost.enabled = false;
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (ghost.enabled && collider.gameObject.CompareTag("Player")) {
            ghostTextBox.SetActive(true);
            ghostTextBox.transform.GetChild(0).GetComponent<TMP_Text>().text = ghostDialogue;
        }
    }

    void OnTriggerExit(Collider collider) {
        if (ghost.enabled && collider.gameObject.CompareTag("Player")) {
            ghostTextBox.SetActive(false);
        }
    }
}
