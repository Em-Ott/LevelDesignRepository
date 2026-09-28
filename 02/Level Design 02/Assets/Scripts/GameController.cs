using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Monster _monster;
    [SerializeField]
    private MouseRaycast _mouseRaycast;
    [SerializeField]
    private Flashlight _flashlight;
    private Action<bool> gameOver;
    private Action<float> batteryChanged;

    void Start()
    {
        batteryChanged += BatteryChanged;
        gameOver += EndGame;

        _flashlight.Init(gameOver, batteryChanged);
        _mouseRaycast.Init(gameOver);
    }

    private void BatteryChanged(float newBatteryPercent)
    {
        _monster.TeleportMonsterToRandomLocation(newBatteryPercent);
    }

    private void EndGame(bool won)
    {
        Debug.Log("ending game");
        _flashlight.GameOver();
        _mouseRaycast.GameOver();

        if (won)
        {
            _monster.PlayerWin();
        }
        else
        {
            _monster.PlayerLoss();
        }
    }
}
