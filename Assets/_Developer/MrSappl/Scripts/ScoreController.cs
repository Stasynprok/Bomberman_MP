using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreController : NetworkBehaviour
{
    [SerializeField] private TMP_Text _score;
    private GameParameters _gameParameters;

    private void OnEnable()
    {
        GameEvents.OnDestroyEnemy += EnemyHandler;
        GameEvents.OnDestroyTile += TileHandler;
        GameEvents.OnPowerUpCollected += CollectionHandler;
    }

    private void OnDisable()
    {
        GameEvents.OnDestroyEnemy -= EnemyHandler;
        GameEvents.OnDestroyTile -= TileHandler;
        GameEvents.OnPowerUpCollected -= CollectionHandler;
    }

    private void Start()
    {
        _score.text = "Score: 0";
        _gameParameters.Score = 0;
    }

    private void UpdateTextScore()
    {
        int score = _gameParameters.Score;
        _score.text = $"Score: {score}";
    }
    private void TileHandler()
    {
        _gameParameters.Score += 50;
        UpdateTextScore();
    }

    private void CollectionHandler()
    {
        _gameParameters.Score += 100;
        UpdateTextScore();
    }
    private void EnemyHandler()
    {
        _gameParameters.Score += 200;
        UpdateTextScore();
    }
}
