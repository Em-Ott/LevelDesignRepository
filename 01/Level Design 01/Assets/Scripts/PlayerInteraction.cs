using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    private Keyboard keyboard;
    private bool alive = true;
    private bool canStab;

    private MeshRenderer knife;

    [SerializeField]
    TMP_Text status;

    void Start() {
        keyboard = Keyboard.current;
        canStab = false;
    }

    void Update() {
        if (keyboard.eKey.wasPressedThisFrame) {
            if ((canStab && alive)) {
                knife.enabled = false;
                ChangeStatus();
            } else if (!alive) {
                knife.enabled = true;
                ChangeStatus();
            }
        }
    }

    void OnTriggerEnter (Collider collider) {
        if (collider.gameObject.CompareTag("Knife")) {
            if (alive) {
                canStab = true;
                knife = collider.gameObject.GetComponent<MeshRenderer>();
            }
        }
    }

    void OnTriggerExit (Collider collider) {
        if (collider.gameObject.CompareTag("Knife")) {
            canStab = false;
        }
    }

    private void ChangeStatus() {
        alive = !alive;
        if (alive) {
            status.text = "Alive";
        } else {
            status.text = "Dead";
        }
    }
}
