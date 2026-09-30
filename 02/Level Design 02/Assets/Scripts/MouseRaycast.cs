using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/*
Note should change this so it isn't a raycast and we just use sphere/some cone?
*/
[Serializable]
public class Protector
{
    public string TagName;
    [System.NonSerialized]
    public bool Found;

    public Protector() { Found = false; }
}

public class MouseRaycast : MonoBehaviour
{
    [SerializeField]
    private Protector[] _requiredTags;
    [SerializeField]
    private int _objectsToFind = 0;
    // Editing actual flashlight size will likely require spotAngle + innerSpotAngle adjustments 
    [SerializeField]
    private Transform _lightTransform;
    [SerializeField]
    private TextMeshProUGUI _foundText;
    [SerializeField]
    private GameObject _flashlight;
    private InputAction _mouseLookAction;
    private string _currentTag = "";
    private int _foundTags = 0;
    private Action<bool> gameWon;
    private bool _gameOver = false;
    private bool _gameStarted = false;

    void Start()
    {
        // See Edit -> Project Settings -> Input Systems Package for actions
        _mouseLookAction = InputSystem.actions.FindAction("Look");
    }

    public void Init(Action<bool> gameOver)
    {
        gameWon = gameOver;
    }

    public void StartGame()
    {
        _gameStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameOver && _gameStarted && _mouseLookAction.WasPerformedThisFrame() && _flashlight.activeSelf)
        {
            _currentTag = MakeRaycast();
            CheckIfCurrentTagRequired();
        }
    }

    public void GameOver()
    {
        _gameOver = true;
    }

    private void CheckIfCurrentTagRequired()
    {
        for (int i = 0; i < _requiredTags.Length; i++)
        {
            if (_requiredTags[i].TagName == _currentTag && !_requiredTags[i].Found)
            {
                _requiredTags[i].Found = true;
                _foundTags++;
                _foundText.text = string.Format("Protectors Found: {0}", _foundTags);

                if (_objectsToFind == _foundTags)
                {
                    gameWon?.Invoke(true);
                    _gameOver = true;
                }
            }
        }
    }

    private string MakeRaycast()
    {
        Ray ray = new Ray(_lightTransform.position, _lightTransform.forward);

        // Spherecasting feels pretty inefficient but feels necessary given this is line of sight based and not a set
        // distance away. Albeit we could just use a huge collider? Doesn't feel efficient either though
        // if (Physics.SphereCast(_light.position, _radius, _light.forward, out RaycastHit hit, _maxDistance))
        // spherecast code ^, issues with displacement for it
        // Issue with below code is there's no radius customization. Other approach is use OverlapSphereNonAlloc
        // to customize radius but concerns about performance. Don't want to get into bad habits optimization wise.
        // Right now raycast alone is probably fine? If art needs more I'll do more
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.DrawLine(GetComponentInChildren<Camera>().gameObject.transform.position, hit.point, Color.green);
            Transform objectHit = hit.transform;
            return objectHit.tag;
        }

        return "";
    }
}
