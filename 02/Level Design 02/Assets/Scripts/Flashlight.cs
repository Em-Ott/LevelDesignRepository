using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Flashlight : MonoBehaviour
{
    [SerializeField]
    private GameObject _flashlight;
    [Tooltip("How many seconds for battery to go down by 1%")]
    [SerializeField]
    private float _batteryDrainSpeed = 1f;
    [SerializeField]
    private TextMeshProUGUI _batteryPercentageText;
    private InputAction _flashlightAction;
    private float _batteryPercentage = 100f;
    private float _counter = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _flashlightAction = InputSystem.actions.FindAction("Flashlight");
        _flashlightAction.performed += _ => ChangeFlashlightVisibility();
    }

    void Update()
    {
        if (_batteryPercentage == 0) { return; }

        _counter += Time.deltaTime;

        if (_counter >= _batteryDrainSpeed)
        {
            _batteryPercentage -= 1;
            _counter = 0;
            _batteryPercentageText.text = string.Format("Battery: {0}%", _batteryPercentage);
        }

        if (_batteryPercentage == 0) { _flashlight.SetActive(false); }
    }

    private void ChangeFlashlightVisibility()
    {
        if (_batteryPercentage == 0) { return; }
        _flashlight.SetActive(!_flashlight.activeSelf);
    }
}
