using UnityEngine;
using UnityEngine.InputSystem;

public class MirrorShard : MonoBehaviour
{
    private bool canPickup;
    private bool holding;
    private bool mirrorRange;
    private bool firstTime;
    private GameObject glass;
    private GameObject mirrorGlass;

    [SerializeField]
    GameObject tutorialText;

    private Keyboard keyboard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyboard = Keyboard.current;
        canPickup = false;
        holding = false;
        mirrorRange = false;
        firstTime = true;
    }

    void Update() {
        if (keyboard.eKey.wasPressedThisFrame) {
            if (canPickup && !holding) {
                glass.SetActive(false);
                holding = true;
                if (firstTime) {
                    tutorialText.SetActive(false);
                    firstTime = false;
                }
            } else if (mirrorRange && holding) {
                mirrorGlass.SetActive(true);
                holding = false;
            }
        }
    }

    void OnTriggerEnter (Collider collider) {
        if (collider.gameObject.CompareTag("Glass")) {
            if (!holding) {
                canPickup = true;
                glass = collider.gameObject;
                mirrorGlass = collider.gameObject.GetComponent<GlassPiece>().GetGlass();
                if (firstTime) {
                    tutorialText.SetActive(true);
                }
            }
        } else if (collider.gameObject.CompareTag("Mirror")) {
            mirrorRange = true;
        }
    }

    void OnTriggerExit (Collider collider) {
        if (collider.gameObject.CompareTag("Glass")) {
            canPickup = false;
        } else if (collider.gameObject.CompareTag("Mirror")) {
            mirrorRange = false;
        }
    }
}
