using UnityEngine;

public class LightController : MonoBehaviour
{
    
    [SerializeField]
    private Light sunlight;
    private float elaspeSpeed;
    private bool isDoingCircle = true;
    private float bigBangThreshold;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDoingCircle)
        {
            sunlight.transform.Rotate(Vector3.right, Time.deltaTime * elaspeSpeed);
        }

        if (sunlight.transform.rotation.eulerAngles.x > bigBangThreshold)
        {
            isDoingCircle = false;
            sunlight.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
