using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : NetworkBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    private GameParameters _gameParameters;

    private void Start()
    {
        //GameParameters.Instance.Timer = 0;
        UpdateTimer();
    }
    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        _timerText.text = $"Timer: {_gameParameters.Timer.ToString("0")}";
    }
}
