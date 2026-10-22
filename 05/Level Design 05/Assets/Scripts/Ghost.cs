using UnityEngine;

public class Ghost : MonoBehaviour
{
    private int phase;

    [SerializeField]
    GameObject ghostLight;

    [SerializeField]
    GameObject ghostForm;

    [SerializeField]
    GameObject brokenMirror;

    [SerializeField]
    GameObject fixedMirror;

    [SerializeField]
    GameObject mirroredPlayer;

    [SerializeField]
    GameObject light;

    private Vector3 target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        phase = 1;
        target = new Vector3(10.21f, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (phase == 3)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 0.5f * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.001f)
            {
                GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<MirrorShard>().Dead();
                phase = 0;
            }
        }
    }

    public void ChangePhase()
    {
        if (phase != 0)
        {
            phase += 1;
            if (phase == 2)
            {
                ghostForm.SetActive(true);
                ghostLight.SetActive(false);
                light.GetComponent<LightFlickering>().ChangeLight(0.2f, 0.7f, 33.0f);
            }
            else if (phase == 3)
            {
                light.GetComponent<LightFlickering>().ChangeLight(0.1f, 0.5f, 66.0f);
            }
            else if (phase == 4)
            {
                brokenMirror.SetActive(false);
                fixedMirror.SetActive(true);
                gameObject.SetActive(false);
                mirroredPlayer.SetActive(true);
                light.GetComponent<LightFlickering>().ChangeLight(10.0f, 10.0f, 100.0f);
                Debug.Log("Should be done.");
            }
        }
    }
}
