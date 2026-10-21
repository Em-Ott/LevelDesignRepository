using UnityEngine;

public class MirrorPlayer : MonoBehaviour
{
    [SerializeField]
    Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(-player.position.x + 5.0f, 1.75544f, player.position.z);

        //t += 0.1f * Time.deltaTime;
    }
}
