using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreController : NetworkBehaviour
{
    [SerializeField] private TMP_Text _score;

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
        GameParameters.Instance.Score = 0;
    }

    private void UpdateTextScore()
    {
        int score = GameParameters.Instance.Score;
        _score.text = $"Score: {score}";
    }
    private void TileHandler()
    {
        GameParameters.Instance.Score += 50;
        UpdateTextScore();
    }

    private void CollectionHandler()
    {
        GameParameters.Instance.Score += 100;
        UpdateTextScore();
    }
    private void EnemyHandler()
    {
        GameParameters.Instance.Score += 200;
        UpdateTextScore();
    }
}
