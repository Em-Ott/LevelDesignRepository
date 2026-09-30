using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GoToBed : MonoBehaviour
{
    [SerializeField]
    private GameObject _bed;
    private Keyboard _keyboard;
    private Action _sleepAction;
    private bool _inBedRange = false;

    void Awake()
    {
        _keyboard = Keyboard.current;
    }

    public void Init(Action sleep)
    {
        _sleepAction = sleep;
    }

    // Update is called once per frame
    void Update()
    {
        if (_keyboard.eKey.isPressed && _inBedRange)
        {
            TeleportToBed();
            _sleepAction?.Invoke();
        }
    }

    private void TeleportToBed()
    {
        this.gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;
        this.gameObject.transform.position = _bed.transform.position + new Vector3(0, 0.9f, 0);
        this.gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
        this.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        this.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "bed")
        {
            _inBedRange = true;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "bed")
        {
            _inBedRange = false;
        }
    }
}
