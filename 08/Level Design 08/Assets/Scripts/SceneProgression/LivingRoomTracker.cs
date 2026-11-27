using System.Collections.Generic;
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
    [SerializeField] GameObject FirstPlay;
    [SerializeField] GameObject SecondPlay;
    [SerializeField] GameObject ThirdPlay;
    [SerializeField] private List<Transform> PlayerPositions;

    private GamePhases Phase;

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
            case GamePhases.Play_First:
                FirstPlay.SetActive(true);
                SecondPlay.SetActive(false);
                ThirdPlay.SetActive(false);
                SceneProgression.ChangePlayerPos(PlayerPositions[0]);
                break;
            case GamePhases.Play_Second:
                FirstPlay.SetActive(false);
                SecondPlay.SetActive(true);
                ThirdPlay.SetActive(false);
                SceneProgression.ChangePlayerPos(PlayerPositions[1]);
                break;
            case GamePhases.Play_Third:
                FirstPlay.SetActive(false);
                SecondPlay.SetActive(false);
                ThirdPlay.SetActive(true);
                SceneProgression.ChangePlayerPos(PlayerPositions[2]);
                break;
        }
    }
}
