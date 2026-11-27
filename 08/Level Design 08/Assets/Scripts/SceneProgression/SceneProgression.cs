using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


/// <summary>
/// Enum of all the phases in the game
/// _L is for scenes that happen in the living room
/// _K is for scenes that happen in the kitchen
/// </summary>
public enum GamePhases
{
    Title,
    InShelter,
    Play_First,
    Eat_First,
    Play_Second,
    Eat_Second,
    Play_Third,
    Eat_Third,
    Death,
    End
}

/// <summary>
/// A script that handles all of the scene progression from title to end.
/// </summary>
public class SceneProgression : MonoBehaviour
{
    public GamePhases Phase { get; private set; }
    public PlayerAge CatAge { get; set; }
    public ObjectToInteractWith TargetObjects { get; set; }
    public ScreenBlink ScreenBlink;

    [SerializeField] private GameObject PlayerObj;


    public void Start()
    {
        Phase = GamePhases.Title;
        ScreenBlink = GetComponent<ScreenBlink>();
        DontDestroyOnLoad(this.gameObject);
    }

    public void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            ChangePhase();
        }
    }

    public void ChangePlayerPos(Transform transform)
    {
        if (PlayerObj == null)
        {
            PlayerObj = GameObject.FindGameObjectWithTag("Player");
        }

        if (PlayerObj != null)
        {
            PlayerObj.transform.position = transform.position;
        }

        Debug.Log($"[SceneProgression] - PlayerObj null?: {PlayerObj == null}");
    }


    public void ToShelter()
    {
        LoadSceneByName("Shelter");
    }

    public void ToLivingRoom()
    {
        LoadSceneByName("Living Room");
    }

    public void ToKitchen()
    {
        LoadSceneByName("Kitchen");
    }

    public void ToEnd()
    {
        // this is optional, don't know if we're going to implement.
    }

    private void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ChangePhase()
    {
        switch (Phase)
        {
            case GamePhases.Title:                      // Check the current GamePhase
                Phase = GamePhases.InShelter;           // Change the current to the next corresponding phase
                ToShelter();                            // Change the Scene
                ScreenBlink.TriggerBlink();
                break;
            case GamePhases.InShelter:
                Phase = GamePhases.Eat_First;
                ToKitchen();
                ScreenBlink.TriggerBlink();
                break;
            case GamePhases.Eat_First:
                Phase = GamePhases.Play_First;
                ToLivingRoom();
                ScreenBlink.TriggerBlink();
                break;
            case GamePhases.Play_First:
                Phase = GamePhases.Eat_Second;
                CatAge.ChangeAge();
                ToKitchen();
                ScreenBlink.TriggerBlink();
                break;
            case GamePhases.Eat_Second:
                Phase = GamePhases.Play_Second;
                ToLivingRoom();
                ScreenBlink.TriggerBlink();
                break;
            case GamePhases.Play_Second:
                Phase = GamePhases.Eat_Third;
                CatAge.GetComponent<PlayerAge>().ChangeAge();
                ToKitchen();
                ScreenBlink.TriggerBlink();
                break;
            case GamePhases.Eat_Third:
                Phase = GamePhases.Play_Third;
                ToLivingRoom();
                ScreenBlink.TriggerBlink();
                break;
            case GamePhases.Play_Third:
                Phase = GamePhases.Death;
                CatAge.GetComponent<PlayerAge>().ChangeAge();
                ToKitchen();
                ScreenBlink.TriggerBlink();
                break;
            case GamePhases.Death:
                Phase = GamePhases.End;
                ToEnd();
                ScreenBlink.TriggerBlink();
                break;
        }

        Debug.Log($"[SceneProgression] - changed to {Phase}");
    }
}
