using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ObjectToInteractWith : MonoBehaviour
{
    public bool CanEatThisItem;
    [SerializeField] private GameObject Key_Interact;
    private SceneProgression Progressor;
    private bool AbleToInteract;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Progressor = GameObject.FindGameObjectWithTag("SceneProgressor").GetComponent<SceneProgression>();
        Progressor.TargetObjects = this;
        AbleToInteract = false;
        if (CanEatThisItem)
        {
            Key_Interact.GetComponent<TMP_Text>().text = "C";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has a specific tag
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger!");
            if (Key_Interact != null)
            {
                Key_Interact.SetActive(true);
                AbleToInteract = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object entering the trigger has a specific tag
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger!");
            if (Key_Interact != null)
            {
                Key_Interact.SetActive(false);
                AbleToInteract = false;
            }
        }
    }

    public void Update()
    {
        if (AbleToInteract)
        {
            if (CanEatThisItem)
            {
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    Progressor.ChangePhase();
                }
            }
            else
            {
                if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
                {
                    Progressor.ChangePhase();
                }
            }

        }

    }

}
