using UnityEngine;

/// <summary>
/// LivingRoomTracker turns on when switching into the Living Room scene.
/// It will check what state the living room should be in, and change the Living Room scene acordingly.
/// All objects that are specific to one scene should be a child inside of a parent object, so that
/// when is it a specific state that living room is, it will turn that parent off, and turn the 
/// according parent activity on.
/// </summary>
public class LivingRoomTracker : MonoBehaviour
{
    public SceneProgression SceneProgression;
    private GamePhases Phase;

    [SerializeField] GameObject DuringCarrier;
    [SerializeField] GameObject DuringPlay;
    [SerializeField] GameObject DuringPetting;
    [SerializeField] GameObject DuringSickness;


    public void Start()
    {
        SceneProgression = GameObject.FindGameObjectWithTag("SceneProgressor").GetComponent<SceneProgression>();
        Phase = SceneProgression.Phase;
        Debug.Log($"[LivingRoomTracker] - phase: {Phase}");
        ChangeObjectByPhase();
    }

    private void ChangeObjectByPhase()
    {
        switch (Phase)
        {
            case GamePhases.InCarrier_L:
                DuringCarrier.SetActive(true);
                DuringPlay.SetActive(false);
                DuringPetting.SetActive(false);
                DuringSickness.SetActive(false);
                break;
            case GamePhases.PlayingWithOwner_L:
                DuringCarrier.SetActive(false);
                DuringPlay.SetActive(true);
                DuringPetting.SetActive(false);
                DuringSickness.SetActive(false);
                break;
            case GamePhases.BeingPet_L:
                DuringCarrier.SetActive(false);
                DuringPlay.SetActive(false);
                DuringPetting.SetActive(true);
                DuringSickness.SetActive(false);
                break;
            case GamePhases.Sick_L:
                DuringCarrier.SetActive(false);
                DuringPlay.SetActive(false);
                DuringPetting.SetActive(false);
                DuringSickness.SetActive(true);
                break;
        }
    }
}
