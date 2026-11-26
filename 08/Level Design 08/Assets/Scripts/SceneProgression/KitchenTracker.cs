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
    private GamePhases Phase;

    [SerializeField] GameObject DuringEating;
    [SerializeField] GameObject DuringThunder;
    [SerializeField] GameObject DuringDeath;


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
            case GamePhases.Dead_K:
                DuringEating.SetActive(false);
                DuringThunder.SetActive(false);
                DuringDeath.SetActive(true);
                break;
            case GamePhases.ScaredOfEating_K:
                DuringEating.SetActive(true);
                DuringThunder.SetActive(false);
                DuringDeath.SetActive(false);
                break;
            case GamePhases.ScaredOfThunder_K:
                DuringEating.SetActive(false);
                DuringThunder.SetActive(true);
                DuringDeath.SetActive(false);
                break;
        }
    }
}
