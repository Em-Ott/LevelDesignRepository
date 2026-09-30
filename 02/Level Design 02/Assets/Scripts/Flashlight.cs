using System;
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
    private Action<bool> GameWon;
    private Action<float> BatteryChanged;
    private bool _gameOver = false;
    private bool _gameStarted = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _flashlightAction = InputSystem.actions.FindAction("Flashlight");
        _flashlightAction.performed += _ => ChangeFlashlightVisibility();
    }

    public void Init(Action<bool> gameOver, Action<float> batteryChnaged)
    {
        GameWon = gameOver;
        BatteryChanged = batteryChnaged;
    }

    public void StartGame()
    {
        _gameStarted = true;
    }

    void Update()
    {
        if (_gameOver || !_gameStarted || !_flashlight.activeSelf) { return; }

        _counter += Time.deltaTime;

        if (_counter >= _batteryDrainSpeed)
        {
            _batteryPercentage -= 1;
            _counter = 0;
            if (_batteryPercentage % 10 == 0) { BatteryChanged?.Invoke(_batteryPercentage); }
            _batteryPercentageText.text = string.Format("Battery: {0}%", _batteryPercentage);
        }

        if (_batteryPercentage == 0)
        {
            _flashlight.SetActive(false);
            GameWon?.Invoke(false);
            _gameOver = true;
        }
    }

    public void GameOver()
    {
        _gameOver = true;
        _flashlight.SetActive(false);
    }

    private void ChangeFlashlightVisibility()
    {
        if (_batteryPercentage == 0 || !_gameStarted || _gameOver) { return; }
        _flashlight.SetActive(!_flashlight.activeSelf);
    }
}
