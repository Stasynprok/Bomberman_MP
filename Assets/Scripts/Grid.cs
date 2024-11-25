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
    private GameParameters _gameParameters;

    private HashSet<string> bs;
    System.Random rnd;

    public override void OnStartServer()
    {
        SetGameParameters();
        if (!_gameParameters)
        {
            return;
        }
        _gameParameters.CountCol = 13;
        _gameParameters.CountRow = 13;
        _gameParameters.CreateArray();

        for (int j = 0; j < 13; j++)
        {
            _gameParameters.SetElementToArray(0, j, 1);
            _gameParameters.SetElementToArray(12, j, 1);
            _gameParameters.SetElementToArray(j, 0, 1);
            _gameParameters.SetElementToArray(j, 12, 1);
        }

        for (int i = 2; i < 12; i += 2)
        {
            for (int j = 2; j < 12; j += 2)
            {
                _gameParameters.SetElementToArray(i, j, 1);
            }
        }

        OnAwake();
        
    }


    [Server]
    private void OnAwake()
    {
        rnd = new System.Random();
        _gameParameters.OriginX = -7.5f;
        _gameParameters.OriginY = 0.5f;
        _gameParameters.OriginZ = 7.5f;

        _gameParameters.Factor = 1;

        _gameParameters.EnemyDestroyed = 0;

        _gameParameters.ToggleMenu = false;
        _gameParameters.GameOver = false;
        

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (_gameParameters.GetElementFromArray(i, j) == 1)
                {
                    /*//GameObject pref = GetObjectFromNetworkManager(hardTile);
                    GameObject obj = InstantiateTile(i, j, hardTile, 0.95f);
                    //NetworkServer.Spawn(obj);*/

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
        _gameParameters.NumberOfTotalEnemies = numberOfEnemy;
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
        Vector3 pos = _gameParameters.MatToWorldPos(x, z);
        pos.y = _y;

        var _tile = Instantiate(tile, pos, tile.transform.rotation);
        _tile.transform.parent = tileContainer.transform;
        return _tile;
    }

    private bool InstantiateDoor() {
        int x = rnd.Next(1, 13);
        int z = rnd.Next(1, 13);

        if (_gameParameters.GetElementFromArray(x, z) != 2 || bs.Contains(x + "," + z)) {
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
        if (_gameParameters.GetElementFromArray(x, z) != 2) {
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
        if (_gameParameters.GetElementFromArray(x, z) == 1 || _gameParameters.GetElementFromArray(x, z) == 2 || (x >= 1 && x <= 5 && z >= 1 && z <= 5)) {
            return false;
        }

        _gameParameters.EnemyX.Add(x);
        _gameParameters.EnemyY.Add(z);

        GameObject pref = GetObjectFromNetworkManager(enemy);
        GameObject obj = InstantiateTile(x, z, pref, 0.95f);
        NetworkServer.Spawn(obj);
        return true;
    }

    private bool InstantiateSoftTile () {
        int x = rnd.Next(1, 13);
        int z = rnd.Next(1, 13);
        if (_gameParameters.GetElementFromArray(x, z) == 1 || _gameParameters.GetElementFromArray(x, z) == 2 || (x >= 1 && x <= 2 && z >= 1 && z <= 2)) {
            return false;
        }

        /* //GameObject pref = GetObjectFromNetworkManager(softTile);
         GameObject obj = InstantiateTile(x, z, softTile, 0.95f);
         //NetworkServer.Spawn(obj);
 */
        GameObject pref = GetObjectFromNetworkManager(softTile);
        GameObject obj = InstantiateTile(x, z, pref, 0.95f);
        NetworkServer.Spawn(obj);

        _gameParameters.SetElementToArray(x, z, 2);
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
        _gameParameters.GameOver = true;
    }

    private void EnemyHandler() {
        _gameParameters.EnemyDestroyed++;
        if (_gameParameters.EnemyDestroyed == _gameParameters.NumberOfTotalEnemies)
        {
            instantiatedGate.GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void InstantiateBombRequestSend() {
        if (_gameParameters.GameOver) {
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

    private void SetGameParameters()
    {
        Scene scene = gameObject.scene;
        _gameParameters = GetGameParametersFromScene(scene);
    }

    private GameParameters GetGameParametersFromScene(Scene scene)
    {
        GameParameters parameters;
        GameObject[] gameObjects;

        gameObjects = scene.GetRootGameObjects();

        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].TryGetComponent<GameParameters>(out GameParameters param))
            {
                parameters = param;
                return parameters;
            }
        }

        return null;
    }
}
