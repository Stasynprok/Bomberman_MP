using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;


public class Grid: NetworkBehaviour
{
    public GameObject hardTile;
    public GameObject softTile;
    public GameObject tileContainer;
    public GameObject bomberman;
    public GameObject enemy;
    public GameObject bomb;
    public GameObject door;
    private GameObject instantiatedGate;

    private HashSet<string> bs;
    System.Random rnd;

    public override void OnStartServer()
    {
        GameParameters.Instance.CountCol = 13;
        GameParameters.Instance.CountRow = 13;
        GameParameters.Instance.CreateArray();

        for (int j = 0; j < 13; j++)
        {
            GameParameters.Instance.SetElementToArray(0, j, 1);
            GameParameters.Instance.SetElementToArray(12, j, 1);
            GameParameters.Instance.SetElementToArray(j, 0, 1);
            GameParameters.Instance.SetElementToArray(j, 12, 1);
        }

        for (int i = 2; i < 12; i += 2)
        {
            for (int j = 2; j < 12; j += 2)
            {
                GameParameters.Instance.SetElementToArray(i, j, 1);
            }
        }

        OnAwake();
        
    }


    [Server]
    private void OnAwake()
    {
        rnd = new System.Random();
        GameParameters.Instance.OriginX = -7.5f;
        GameParameters.Instance.OriginY = 0.5f;
        GameParameters.Instance.OriginZ = 7.5f;

        GameParameters.Instance.Factor = 1;

        GameParameters.Instance.EnemyDestroyed = 0;

        GameParameters.Instance.ToggleMenu = false;
        GameParameters.Instance.GameOver = false;
        

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (GameParameters.Instance.GetElementFromArray(i, j) == 1)
                {
                    GameObject pref = GetObjectFromNetworkManager(hardTile);
                    GameObject obj = InstantiateTile(i, j, pref, 0.95f);
                    NetworkServer.Spawn(obj);
                }
            }
        }

         int numberOfSoftTile = rnd.Next(25, 36);
        while (numberOfSoftTile > 0)
        {
            if (InstantiateSoftTile())
            {
                numberOfSoftTile--;
            }
        }

        int numberOfEnemy = rnd.Next(5, 10);
        GameParameters.Instance.NumberOfTotalEnemies = numberOfEnemy;
        while (numberOfEnemy > 0)
        {
            if (InstantiateEnemy())
            {
                numberOfEnemy--;
            }
        }

        int numberOfCollectible = rnd.Next(2, 6);
        bs = new HashSet<string>();
        while (numberOfCollectible > 0)
        {
            if (InstantiateCollectible())
            {
                numberOfCollectible--;
            }
        }

        bool doorInstantiated = false;
        while (!doorInstantiated)
        {
            if (InstantiateDoor())
            {
                doorInstantiated = true;
            }
        }
    }

    private GameObject InstantiateTile (int x, int z, GameObject tile, float _y = 0.5f) {
        Vector3 pos = GameParameters.Instance.MatToWorldPos(x, z);
        pos.y = _y;

        var _tile = Instantiate(tile, pos, tile.transform.rotation);
        _tile.transform.parent = tileContainer.transform;
        return _tile;
    }

    private bool InstantiateDoor() {
        int x = rnd.Next(1, 13);
        int z = rnd.Next(1, 13);

        if (GameParameters.Instance.GetElementFromArray(x, z) != 2 || bs.Contains(x + "," + z)) {
            return false;
        }

        GameObject pref = GetObjectFromNetworkManager(door);
        GameObject obj = InstantiateTile(x, z, pref, 0.95f);
        NetworkServer.Spawn(obj);
        return true;
    }

    private bool InstantiateCollectible () {
        int x = rnd.Next(1, 13);
        int z = rnd.Next(1, 13);
        if (GameParameters.Instance.GetElementFromArray(x, z) != 2) {
            return false;
        }

        GameObject pref = GetObjectFromNetworkManager(bomb);
        GameObject obj = InstantiateTile(x, z, pref, 0.95f);
        NetworkServer.Spawn(obj);
        bs.Add(x + "," + z);
        return true;
    }

    private bool InstantiateEnemy () {
        int x = rnd.Next(1, 13);
        int z = rnd.Next(1, 13);
        if (GameParameters.Instance.GetElementFromArray(x, z) == 1 || GameParameters.Instance.GetElementFromArray(x, z) == 2 || (x >= 1 && x <= 5 && z >= 1 && z <= 5)) {
            return false;
        }

        GameParameters.Instance.EnemyX.Add(x);
        GameParameters.Instance.EnemyY.Add(z);

        GameObject pref = GetObjectFromNetworkManager(enemy);
        GameObject obj = InstantiateTile(x, z, pref, 0.95f);
        NetworkServer.Spawn(obj);
        return true;
    }

    private bool InstantiateSoftTile () {
        int x = rnd.Next(1, 13);
        int z = rnd.Next(1, 13);
        if (GameParameters.Instance.GetElementFromArray(x, z) == 1 || GameParameters.Instance.GetElementFromArray(x, z) == 2 || (x >= 1 && x <= 2 && z >= 1 && z <= 2)) {
            return false;
        }

        GameObject pref = GetObjectFromNetworkManager(softTile);
        GameObject obj = InstantiateTile(x, z, pref, 0.95f);
        NetworkServer.Spawn(obj);

        GameParameters.Instance.SetElementToArray(x, z, 2);
        return true;
    }

    private void OnEnable()
    {
        GameEvents.OnGateEnter += GateHandler;
        GameEvents.OnDestroyEnemy += EnemyHandler;
        GameEvents.OnDestroyBomberman += GateHandler;
    }

    private void OnDisable()
    {
        GameEvents.OnGateEnter -= GateHandler;
        GameEvents.OnDestroyEnemy -= EnemyHandler;
        GameEvents.OnDestroyBomberman -= GateHandler;
    }

    private void GateHandler() {
        GameParameters.Instance.GameOver = true;
    }

    private void EnemyHandler() {
        GameParameters.Instance.EnemyDestroyed++;
        if (GameParameters.Instance.EnemyDestroyed == GameParameters.Instance.NumberOfTotalEnemies)
        {
            instantiatedGate.GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void InstantiateBombRequestSend() {
        if (GameParameters.Instance.GameOver) {
            return;
        }

        GameEvents.OnRequestInvoke();
    }

    private GameObject GetObjectFromNetworkManager(GameObject gameObject)
    {
        for (int i = 0; i < NetworkManager.singleton.spawnPrefabs.Count; i++)
        {
            if (NetworkManager.singleton.spawnPrefabs[i] == gameObject)
            {
                return NetworkManager.singleton.spawnPrefabs[i];
            }
        }
        return null;
    }
}
