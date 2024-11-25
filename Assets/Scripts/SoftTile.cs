using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoftTile : NetworkBehaviour
{
    private Vector2Int cord;
    public ParticleSystem p;
    private GameParameters _gameParameters;

    private void Start()
    {
        if (isServer)
        {
            SetGameParameters();
        }

        if (isClient)
        {
            Debug.Log("Trysss");
            SetGameParametersLocal();
        }

        if (!_gameParameters)
        {
            return;
        }
        cord = _gameParameters.PosToMat(transform.position);
    }
    
    private void OnEnable()
    {
        GameEvents.OnDestroyBomb += SelfDestroyHandler;
    }

    
    private void OnDestroy()
    {
        GameEvents.OnDestroyBomb -= SelfDestroyHandler;
        _gameParameters.SetElementToArray(cord.x, cord.y, 0);
    }
    
    private void SelfDestroyHandler(Vector2Int cord) {
        if (!_gameParameters.SafeFromBomb(cord, this.cord)) {
            //Debug.Log("Tile Destroyed");
            GameEvents.OnDestroyTileInvoke();
            Instantiate(p, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }

    
    private void SetGameParametersLocal()
    {
        Debug.Log("Try");
        _gameParameters = FindFirstObjectByType<GameParameters>();
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
