using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : NetworkBehaviour
{
    private int x;
    private int z;
    private bool isMoving;
    int[,] move;
    private Vector3 rotationAngles;
    System.Random rnd;
    private GameParameters _gameParameters;

    private void Start()
    {
        if (!isServer)
        {
            return;
        }
        SetGameParameters();
        x = z = 1;
        isMoving = false;
        move = new int[,] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
        rnd = new System.Random();

        Vector2Int pos = _gameParameters.GetCoordinate();
        //Debug.Log($"pos is {pos}");
        x = pos.x;
        z = pos.y;
    }


    private void OnEnable()
    {
        GameEvents.OnDestroyBomb += SelfDestroyHandler;
    }

    private void OnDisable()
    {
        GameEvents.OnDestroyBomb -= SelfDestroyHandler;
        
    }

    private void SelfDestroyHandler(Vector2Int cord) {
        if (!_gameParameters.SafeFromBomb(cord, new Vector2Int(x, z))) {
            //Debug.Log("Enemy Destroyed");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnDestroyEnemyInvoke();
        SoundManagement.sm.PlayHurt();
    }

    private void FixedUpdate()
    {
        if (!isServer)
        {
            return;
        }

        int i = rnd.Next(0, 4);
        Vector2Int next = new (move[i, 0], move[i, 1]);
        OnMovementPerformed(next);
    }

    private void OnMovementPerformed(Vector2Int temp) {
        Vector2Int newPos = new(x + temp.x, z + temp.y);
        if (!isMoving && _gameParameters.CanMove(newPos)) {
            Vector3 dest = _gameParameters.MatToWorldPos(newPos.x, newPos.y);
            //float y_rotation = 0;
            if (temp.y == 1) {
                rotationAngles.y = 90;
            } else if (temp.y == -1) {
                rotationAngles.y = -90;
            } else if (temp.x == 1) {
                rotationAngles.y = 180;
            } else {
                rotationAngles.y = 0;
            }

            //InstantiateFootSteps(y_rotation);
            StartCoroutine(Move(dest));
            x = newPos.x;
            z = newPos.y;
        }
    }

    private IEnumerator Move(Vector3 dest)
    {
        isMoving = true;
        //Vector3 end = transform.position + dir, start = transform.position;
        Vector3 end = dest, start = transform.position;
        end.y = 0.95f;
        float timeElapsed = 0;
        float timeToMove = 0.8f;


        while (timeElapsed < timeToMove)
        {
            transform.position = Vector3.Lerp(start, end, timeElapsed / timeToMove);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationAngles), timeElapsed / timeToMove);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isMoving = false;
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
    /*private void InstantiateFootSteps(float y) {
        var obj = _poolFoots.GetObjectFromPool();
        obj.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
        obj.transform.rotation = Quaternion.Euler(90, y, 0);

        FootStepScript step = obj.GetComponent<FootStepScript>();

        step.Initialize(_poolFoots);
    }*/
}
