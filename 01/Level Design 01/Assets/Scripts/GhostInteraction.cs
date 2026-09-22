using UnityEngine;

public class GhostInteraction : MonoBehaviour
{
    [SerializeField]
    MeshRenderer decadeKnife;
    MeshRenderer ghost;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ghost = gameObject.GetComponent<MeshRenderer>();
        ghost.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!decadeKnife.enabled) {
            ghost.enabled = true;
        } else {
            ghost.enabled = false;
        }
    }
}
