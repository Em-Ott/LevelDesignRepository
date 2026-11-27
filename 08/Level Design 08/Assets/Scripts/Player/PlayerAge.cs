using UnityEngine;

public enum Age { Kitten, Adult, Sick, Dead }
public class PlayerAge : MonoBehaviour
{
    public Age Age { get; private set; }
    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        GameObject.FindGameObjectWithTag("SceneProgressor").GetComponent<SceneProgression>().CatAge = this;
        ChangeAge(Age.Kitten);
    }

    private void ChangeAge(Age age)
    {
        Age = age;
        Debug.Log($"[PlayerAge] - Age: {Age}");

    }

    public void ChangeAge()
    {
        switch (Age)
        {
            case Age.Kitten:
                ChangeAge(Age.Adult);
                break;
            case Age.Adult:
                ChangeAge(Age.Sick);
                break;
            case Age.Sick:
                ChangeAge(Age.Dead);
                break;
            case Age.Dead:
                Debug.Log($"[PlayerAge] - Cat died");
                break;
        }

        Debug.Log($"[PlayerAge] - Age: {Age}");

    }

    void Update()
    {

    }
}
