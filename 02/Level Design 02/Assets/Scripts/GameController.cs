using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Monster _monster;
    [SerializeField]
    private PlayerMovement _playerMovement;
    [SerializeField]
    private PlayerLook _playerLook;
    [SerializeField]
    private MouseRaycast _mouseRaycast;
    [SerializeField]
    private Flashlight _flashlight;
    [SerializeField]
    private GoToBed _sleep;
    private Action<bool> gameOver;
    private Action<float> batteryChanged;
    private Action gameStart;

    void Start()
    {
        batteryChanged += BatteryChanged;
        gameOver += (bool won) => _ = EndGame(won);
        gameStart += StartGame;

        _flashlight.Init(gameOver, batteryChanged);
        _mouseRaycast.Init(gameOver);
        _sleep.Init(gameStart);

        _playerLook.enabled = false;
    }

    void Update()
    {

    }

    private void StartGame()
    {
        _playerMovement.enabled = false;
        _sleep.enabled = false;
        _playerLook.enabled = true;
        _flashlight.StartGame();
        _mouseRaycast.StartGame();
        _monster.ShowMonster();
    }

    private void BatteryChanged(float newBatteryPercent)
    {
        _monster.TeleportMonsterToRandomLocation(newBatteryPercent);
    }

    private async Task EndGame(bool won)
    {
        _flashlight.GameOver();
        _mouseRaycast.GameOver();

        if (won)
        {
            _monster.PlayerWin();
        }
        else
        {
            _monster.PlayerLoss();
            await Awaitable.WaitForSecondsAsync(2f);
            _monster.PlayerWin();
        }

        _playerLook.enabled = false;
        _playerMovement.enabled = true;
        _playerMovement.FixAngles();
    }
}
