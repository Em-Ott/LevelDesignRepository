using UnityEngine;

public class Ghost : MonoBehaviour
{
    private int phase;
    public float start = -1.0F;
    public float end =  1.0F;
    static float t = 0.0f;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        phase = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (phase == 3) {
            transform.position = new Vector3(0.97974f, 2.54321f, 1.02928f + Mathf.Lerp(start, end, t));

            t += 0.1f * Time.deltaTime;
            if (t > 1.0f)
            {
                GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<MirrorShard>().Dead();
                phase = 0;
            }
        }
    }

    public void ChangePhase() {
        if (phase != 0) {
            phase += 1;
            if (phase == 2) {
                ghostForm.SetActive(true);
                ghostLight.SetActive(false);
                light.GetComponent<LightFlickering>().ChangeLight(0.2f, 0.7f, 0.5f);
            } else if (phase == 3) {
                light.GetComponent<LightFlickering>().ChangeLight(0.1f, 0.5f, 1.0f);
            } else if (phase == 4) {
                brokenMirror.SetActive(false);
                fixedMirror.SetActive(true);
                gameObject.SetActive(false);
                mirroredPlayer.SetActive(true);
                light.GetComponent<LightFlickering>().ChangeLight(10.0f, 10.0f, 0.5f);
            }
        }
    }
}
