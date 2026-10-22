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
    private bool alive;
    private bool toiletAvailable;
    private bool canToilet;

    [SerializeField]
    GameObject tutorialText;

    [SerializeField]
    GameObject ghost;

    [SerializeField]
    GameObject deadText;

    [SerializeField]
    GameObject toiletDown;

    [SerializeField]
    GameObject toiletUp;

    [SerializeField]
    GameObject toiletShard;

    private Keyboard keyboard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyboard = Keyboard.current;
        canPickup = false;
        holding = false;
        mirrorRange = false;
        firstTime = true;
        alive = true;
        canToilet = false;
        toiletAvailable = true;
    }

    void Update() {
        if (keyboard.eKey.wasPressedThisFrame && alive) {
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
                ghost.GetComponent<Ghost>().ChangePhase();
            } else if (canToilet) {
                toiletDown.SetActive(false);
                toiletUp.SetActive(true);
                toiletShard.SetActive(true);
                toiletAvailable = false;
                canToilet = false;
            }
        }
    }

    void OnTriggerEnter (Collider collider) {
        if (collider.gameObject.CompareTag("Glass") && alive) {
            if (!holding) {
                canPickup = true;
                glass = collider.gameObject;
                mirrorGlass = collider.gameObject.GetComponent<GlassPiece>().GetGlass();
                if (firstTime) {
                    tutorialText.SetActive(true);
                }
            }
        } else if (collider.gameObject.CompareTag("Mirror") && alive) {
            mirrorRange = true;
        } else if (collider.gameObject.CompareTag("Toilet") && toiletAvailable) {
            canToilet = true;
        }
    }

    void OnTriggerExit (Collider collider) {
        if (collider.gameObject.CompareTag("Glass") && alive) {
            canPickup = false;
        } else if (collider.gameObject.CompareTag("Mirror") && alive) {
            mirrorRange = false;
        } else if (collider.gameObject.CompareTag("Toilet") && toiletAvailable) {
            canToilet = false;
        }
    }

    public void Dead() {
        alive = false;
        deadText.SetActive(true);
    }
}
