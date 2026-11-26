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
    InCarrier_L,
    ScaredOfEating_K,
    PlayingWithOwner_L,
    BeingPet_L,
    ScaredOfThunder_K,
    Sick_L,
    Dead_K,
    End
}

/// <summary>
/// A script that handles all of the scene progression from title to end.
/// </summary>
public class SceneProgression : MonoBehaviour
{
    public GamePhases Phase { get; private set; }
    // public GameObject L_TrackerObject
    // public GameObject K_TrackerObject

    // private LivingRoomTracker L_Phase;
    // private KitchenTracker K_Phase;
    public void Start()
    {
        Phase = GamePhases.Title;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            ChangePhase();
        }
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
                break;
            case GamePhases.InShelter:
                Phase = GamePhases.InCarrier_L;
                ToLivingRoom();
                break;
            case GamePhases.InCarrier_L:
                Phase = GamePhases.ScaredOfEating_K;
                ToKitchen();
                break;
            case GamePhases.ScaredOfEating_K:
                Phase = GamePhases.PlayingWithOwner_L;
                ToLivingRoom();
                break;
            case GamePhases.PlayingWithOwner_L:
                Phase = GamePhases.BeingPet_L;
                ToLivingRoom();
                break;
            case GamePhases.BeingPet_L:
                Phase = GamePhases.ScaredOfThunder_K;
                ToKitchen();
                break;
            case GamePhases.ScaredOfThunder_K:
                Phase = GamePhases.Sick_L;
                ToLivingRoom();
                break;
            case GamePhases.Sick_L:
                Phase = GamePhases.Dead_K;
                ToKitchen();
                break;
            case GamePhases.Dead_K:
                Phase = GamePhases.End;
                ToEnd();
                break;
        }

        Debug.Log($"[SceneProgression] - changed to {Phase}");
    }
}
