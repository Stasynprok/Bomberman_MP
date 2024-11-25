using Mirror;
using UnityEngine;
using System.Numerics;
using System;
using System.Collections.Generic;

public class GameParameters : NetworkBehaviour
{
    public GameObject TileContainer;
    [SyncVar]
    public float OriginX;
    [SyncVar]
    public float OriginY;
    [SyncVar]
    public float OriginZ;

    [SyncVar]
    public List<int> EnemyX = new List<int>();
    [SyncVar]
    public List<int> EnemyY = new List<int>();

    [SyncVar]
    public int Factor;
    [SyncVar]
    public int EnemyDestroyed, NumberOfTotalEnemies;

    [SyncVar]
    public float Timer;

    [SyncVar]
    public int Score;
    [SyncVar]
    public bool ToggleMenu, GameOver;

    [SyncVar]
    public int CountRow;
    [SyncVar]
    public int CountCol;

    [SyncVar]
    public int[] Mat1d;

/*    #region Singleton
    /// <summary>
    /// Instance of our Singleton
    /// </summary>
    public static GameParameters Instance
    {
        get
        {
            return _instance;
        }
    }
    private static GameParameters _instance;

    public void InitializeSingletonGameParameters()
    {
        // Destroy any duplicate instances that may have been created
        if (_instance != null && _instance != this)
        {
            Debug.Log("destroying singleton");
            Destroy(this);
            return;
        }
        _instance = this;
    }
    private void Awake()
    {
        InitializeSingletonGameParameters();
    }
    #endregion*/

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < CountRow; i++)
        {
            for (int j = 0; j < CountCol; j++)
            {
                if (GetElementFromArray(j, i) == 0)
                {
                    Debug.DrawRay(MatToWorldPos(j, i), UnityEngine.Vector3.up, Color.red);

                }
                if (GetElementFromArray(j, i) == 1)
                {
                    Debug.DrawRay(MatToWorldPos(j, i), UnityEngine.Vector3.up, Color.green);

                }
                if (GetElementFromArray(j, i) == 2)
                {
                    Debug.DrawRay(MatToWorldPos(j, i), UnityEngine.Vector3.up, Color.black);

                }
            }
        }
    }


    public void InitBomberman(GameObject tile)
    {
        UnityEngine.Vector3 pos = MatToWorldPos(1, 1);
        pos.y = 0.5f;

        tile.transform.position = pos;
        tile.transform.parent = TileContainer.transform;
    }

    public UnityEngine.Vector3 MatToWorldPos(int x, int z)
    {
        float _x = z;
        float _z = -x;

        return new UnityEngine.Vector3(_x + OriginX, OriginY, _z + OriginZ);
    }

    public Vector2Int PosToMat(UnityEngine.Vector3 pos)
    {
        Vector2Int p = new(0, 0);

        float x = pos.x - OriginX;
        float z = pos.z - OriginZ;
        p.x = -(int)z;
        p.y = (int)x;
        return p;
    }

    public bool SafeFromBomb(Vector2Int bomb, Vector2Int obj)
    {
        Vector2Int up = new(-1, 0);
        Vector2Int down = new(1, 0);
        Vector2Int left = new(0, -1);
        Vector2Int right = new(0, 1);

        Vector2Int pos = bomb;
        for (int i = 0; i < Factor; i++)
        {
            pos += up;
            if (pos == obj)
            {
                return false;
            }
            else if (pos.x < 1 || GetElementFromArray(pos.x, pos.y) == 1)
            {
                break;
            }
        }

        pos = bomb;
        for (int i = 0; i < Factor; i++)
        {
            pos += down;
            if (pos == obj)
            {
                return false;
            }
            else if (pos.x > 11 || GetElementFromArray(pos.x, pos.y) == 1)
            {
                break;
            }
        }

        pos = bomb;
        for (int i = 0; i < Factor; i++)
        {
            pos += left;
            if (pos == obj)
            {
                return false;
            }
            else if (pos.y < 1 || GetElementFromArray(pos.x, pos.y) == 1)
            {
                break;
            }
        }

        pos = bomb;
        for (int i = 0; i < Factor; i++)
        {
            pos += right;
            if (pos == obj)
            {
                return false;
            }
            else if (pos.y > 11 || GetElementFromArray(pos.x, pos.y) == 1)
            {
                break;
            }
        }

        return obj != bomb;
    }
    public bool CanMove(Vector2Int pos)
    {
        bool canMove = pos.x >= 0 && pos.x < 13 && pos.y >= 0 && pos.y < 13 && GetElementFromArray(pos.x, pos.y) != 1 && GetElementFromArray(pos.x, pos.y) != 2;
        return canMove;
    }

    public int GetElementFromArray(int x, int y)
    {
        int element = Mat1d[y * CountCol + x];
        return element;
    }
    public void CreateArray()
    {
        Mat1d = new int[CountCol * CountRow];
    }

    public void SetElementToArray(int x, int y, int value)
    {
        Mat1d[y * CountCol + x] = value;
    }

    public Vector2Int GetCoordinate()
    {
        Vector2Int cord = Vector2Int.zero;
        if (EnemyX.Count > 0 && EnemyY.Count > 0)
        {
            cord.x = EnemyX[0];
            cord.y = EnemyY[0];

            EnemyX.RemoveAt(0);
            EnemyY.RemoveAt(0);
        }

        return cord;
    }
}
