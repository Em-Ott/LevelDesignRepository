using UnityEngine;
using UnityEngine.SceneManagement;
public enum GamePhases
{
    InShelter,
    InCarrier_L,
    ScaredOfEating_K,
    PlayingWithOwner_L,
    BeingPet_L,
    ScaredOfThunder_K,
    Sick_L,
    Dead_K,
    Title
}

public class SceneProgression : MonoBehaviour
{
    public GamePhases Phase { get; private set; }

    void Start()
    {
        Phase = GamePhases.Title;
    }

    public void PressPlay()
    {
        ChangePhase();
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
        }

        Debug.Log($"[SceneProgression] - changed to {Phase}");
    }
}
