using UnityEngine;

/*
Teleports the monster to a random location once every 10%s.
Has different locations for different battery levels getting increasingly closer
At 0 teleports on top of the player
*/
public class Monster : MonoBehaviour
{
    [SerializeField]
    private GameObject _monster;
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private Vector3[] _highBatteryLocations;
    [SerializeField]
    private Vector3[] _mediumBatteryLocations;
    [SerializeField]
    private Vector3[] _lowBatteryLocations;
    [SerializeField]
    private float _highBatteryMinimum = 65f;
    [SerializeField]
    private float _mediumBatteryMinimum = 30f;

    public void TeleportMonsterToRandomLocation(float batteryPercentage)
    {
        Vector3 newPosition = new Vector3(0, 0, 0);
        if (batteryPercentage >= _highBatteryMinimum)
        {
            int rand = Random.Range(0, _highBatteryLocations.Length);
            newPosition = _highBatteryLocations[rand];
        }
        else if (batteryPercentage >= _mediumBatteryMinimum)
        {
            int rand = Random.Range(0, _mediumBatteryLocations.Length);
            newPosition = _mediumBatteryLocations[rand];
        }
        else
        {
            int rand = Random.Range(0, _lowBatteryLocations.Length);
            newPosition = _lowBatteryLocations[rand];
        }
        _monster.transform.localPosition = newPosition;
    }

    public void ShowMonster()
    {
        _monster.SetActive(true);
    }

    public void PlayerLoss()
    {
        _monster.transform.position = _player.transform.position + new Vector3(0, 0, 1.75f);
    }

    public void PlayerWin()
    {
        _monster.SetActive(false);
    }
}
