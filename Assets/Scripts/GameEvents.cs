using Mirror;
using System;
using UnityEngine;

public class GameEvents : NetworkBehaviour
{
    public static event Action OnDestroyEnemy;
    public static event Action OnDestroyBomberman;
    public static event Action OnDestroyTile;

    public static event Action OnPowerUpCollected;
    public static event Action OnGateEnter;
    public static event Action OnRequest;

    public static event Action<Vector2Int> OnDestroyBomb;

    public static void OnDestroyBombInvoke(Vector2Int cord) {
        OnDestroyBomb?.Invoke(cord);
    }

    public static void OnDestroyEnemyInvoke() {
        OnDestroyEnemy?.Invoke();
    }

    public static void OnDestroyBombermanInvoke() {
        OnDestroyBomberman?.Invoke();
    }

    public static void OnDestroyTileInvoke() {
        OnDestroyTile?.Invoke();
    }

    public static void OnPowerUpCollectedInvoke() {
        OnPowerUpCollected?.Invoke();
    }

    public static void OnGateEnterInvoke() {
        OnGateEnter?.Invoke();
    }

    public static void OnRequestInvoke() {
        OnRequest?.Invoke();
    }
}
