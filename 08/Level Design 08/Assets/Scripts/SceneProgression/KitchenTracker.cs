using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// KitchenTracker turns on when switching into the Kitchen scene.
/// It will check what state the Kitchen should be in, and change the Kitchen scene acordingly.
/// All objects that are specific to one scene should be a child inside of a parent object, so that
/// when is it a specific state that Kitchen is, it will turn that parent off, and turn the 
/// according parent activity on.
/// </summary>
public class KitchenTracker : MonoBehaviour
{
    public SceneProgression SceneProgression;
    [SerializeField] private GameObject FirstEat;
    [SerializeField] private GameObject SecondEat;
    [SerializeField] private GameObject ThirdEat;
    [SerializeField] private GameObject DeathScene;

    [SerializeField] private List<Transform> PlayerPositions;
    private GamePhases Phase;


    public void Start()
    {
        SceneProgression = GameObject.FindGameObjectWithTag("SceneProgressor").GetComponent<SceneProgression>();
        Phase = SceneProgression.Phase;
        Debug.Log($"[Kitchen] - phase: {Phase}");
        ChangeObjectByPhase();
    }

    private void ChangeObjectByPhase()
    {
        switch (Phase)
        {
            case GamePhases.Eat_First:
                FirstEat.SetActive(true);
                SecondEat.SetActive(false);
                ThirdEat.SetActive(false);
                SceneProgression.ChangePlayerPos(PlayerPositions[0]);
                break;
            case GamePhases.Eat_Second:
                FirstEat.SetActive(false);
                SecondEat.SetActive(true);
                ThirdEat.SetActive(false);
                SceneProgression.ChangePlayerPos(PlayerPositions[1]);
                break;
            case GamePhases.Eat_Third:
                FirstEat.SetActive(false);
                SecondEat.SetActive(false);
                ThirdEat.SetActive(true);
                SceneProgression.ChangePlayerPos(PlayerPositions[2]);
                break;
            case GamePhases.Death:
                FirstEat.SetActive(false);
                SecondEat.SetActive(false);
                ThirdEat.SetActive(false);
                SceneProgression.ChangePlayerPos(PlayerPositions[3]);
                break;
        }
    }
}
