using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    private Keyboard keyboard;
    private bool alive = true;
    private bool canStab;
    //private int decade;

    private GameObject knife;

    [SerializeField]
    TMP_Text status;

    void Start() {
        keyboard = Keyboard.current;
        canStab = false;
        //decade = 0;
    }

    void Update() {
        if (keyboard.eKey.wasPressedThisFrame) {
            if ((canStab && alive)) {
                knife.SetActive(false);
                ChangeStatus();
                canStab = false;
            } else if (!alive) {
                knife.SetActive(true);
                ChangeStatus();
            }
        }
    }

    void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.CompareTag("Knife")) {
            if (alive) {
                canStab = true;
                knife = collision.gameObject;
            }
        }
    }

    void OnCollisionExit (Collision collision) {
        if (collision.gameObject.CompareTag("Knife")) {
            if (alive) {
                canStab = false;
            }
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
