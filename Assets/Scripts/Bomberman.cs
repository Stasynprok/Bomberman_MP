using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class Bomberman : NetworkBehaviour
{
    [SerializeField] private TMP_Text _nicknameText;
    private CustomMovement input;
    private int x;
    private int z;
    private bool isMoving;
    private Vector3 rotationAngles;
    public Animator anim;
    public GameObject bomb;

    private int maxBombs;
    private int spawnedBombs;
    private Vector3 position, rotation, scale;
    public float dur;
    //public GameObject footStep;

    //private FootStepPool _poolFoots;
    [SyncVar(hook = nameof(SetNick))]
    private string Nick;
    public GameParameters GameParameters;

    private void Awake()
    {
        x = z = 1;
        isMoving = false;
        maxBombs = 1;
        spawnedBombs = 0;
        input = new CustomMovement();
        position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        scale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //_poolFoots = GetComponent<FootStepPool>();
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        SetGameParameters();
        SetBomberman();
    }
    public void SetBomberman()
    {
        GameParameters grid = GameParameters;

        grid.InitBomberman(gameObject);
    }

    private void OnEnable()
    {
        input.Enable();
        input.Bomberman.Movement.performed += OnMovementPerformed;
        input.Bomberman.Attack.performed += InstantiateBomb;
        GameEvents.OnRequest += InstantiateBombHandler;
        GameEvents.OnDestroyBomb += SelfDestroyHandler;
    }

    private void OnDisable()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        input.Bomberman.Movement.performed -= OnMovementPerformed;
        input.Disable();
        GameEvents.OnDestroyBomb -= SelfDestroyHandler;
        GameEvents.OnRequest -= InstantiateBombHandler;
        GameEvents.OnDestroyBombermanInvoke();
        SoundManagement.sm.PlayHurt();
    }

    private void SelfDestroyHandler(Vector2Int cord) {
        if (!isLocalPlayer)
        {
            return;
        }
        if (!GameParameters.SafeFromBomb(cord, new Vector2Int(x, z))) {
            //Debug.Log("Bomberman Destroyed");
            Destroy(gameObject);
        } else {
            spawnedBombs--;
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        anim.SetBool("Walk", isMoving);
        anim.SetBool("Idle", !isMoving);
    }

    private void InstantiateBomb(InputAction.CallbackContext _v) {
        if (!isLocalPlayer)
        {
            return;
        }
        InstantiateBombHandler();
    }

    public void InstantiateBombHandler() {
        if (!isLocalPlayer)
        {
            return;
        }
        if (spawnedBombs == maxBombs)
        {
            return;
        }

        //Debug.Log("Called");
        spawnedBombs++;
        Instantiate(bomb, transform.position, transform.rotation);
    }

    private void OnMovementPerformed(InputAction.CallbackContext _v) {
        if (!isLocalPlayer)
        {
            return;
        }
        Debug.Log($"[OnMovementPerformed] isLocalPlayer: {isLocalPlayer}");
        Vector2 val = _v.ReadValue<Vector2>();
        OnMovementPerformedUtil(val);
    }

    private void OnMovementPerformedUtil(Vector2 val) {
        if (!isLocalPlayer)
        {
            return;
        }
        Vector2Int temp = new(Mathf.RoundToInt(val.x), Mathf.RoundToInt(val.y));

        Vector2Int newPos = new(x - temp.y, z + temp.x);
        if (!isMoving && GameParameters.CanMove(newPos)) {
            Vector3 dest = GameParameters.MatToWorldPos(newPos.x, newPos.y);
            //float y_rotation = 0;
            if (temp.x == -1) {
                rotationAngles.y = -90;
            } else if (temp.x == 1) {
                rotationAngles.y = 90;
            } else if (temp.y == 1) {
                rotationAngles.y = 0;
            } else {
                rotationAngles.y = 180;
            }

            //InstantiateFootSteps(y_rotation);

            StartCoroutine(Move(dest));
            x = newPos.x;
            z = newPos.y;
        }
    }

    private IEnumerator Move(Vector3 dest)
    {
        if (!isLocalPlayer)
        {
            yield break;
        }
        isMoving = true;
        SoundManagement.sm.PlayFootStep();
        //Vector3 end = transform.position + dir, start = transform.position;
        Vector3 end = dest, start = transform.position;
        end.y = 0;
        float timeElapsed = 0;
        float timeToMove = 0.2f;


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

    private void OnCollisionEnter(Collision collision)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (collision.collider.CompareTag("enemy")) {
            Destroy(gameObject);
        } else if (collision.collider.CompareTag("powerUp")) {
            //Debug.Log("Collected powerup");
            GameEvents.OnPowerUpCollectedInvoke();
            SoundManagement.sm.PlayCollectible();
            Destroy(collision.collider.transform.gameObject);
            maxBombs++;
        } else if (collision.collider.CompareTag("door")) {
            gameObject.transform.position = position;
            gameObject.transform.rotation = Quaternion.Euler(rotation);
            gameObject.transform.localScale = scale;
            SoundManagement.sm.PlayCollectible();
            GameEvents.OnGateEnterInvoke();
        }
    }

    /*    private void InstantiateFootSteps(float y) {
            if (!isLocalPlayer)
            {
                return;
            }

            var obj = _poolFoots.GetObjectFromPool();
            obj.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
            obj.transform.rotation = Quaternion.Euler(90, y, 0);

            FootStepScript step = obj.GetComponent<FootStepScript>();

            step.Initialize(_poolFoots);
        }*/
    [Client]
    private void SetGameParameters()
    {
        Scene scene = gameObject.scene;
        GameParameters = FindFirstObjectByType<GameParameters>();
        //GameParameters = GetGameParametersFromScene(scene);
    }

    /*private GameParameters GetGameParametersFromScene(Scene scene)
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
    }*/
    public void SetNickname(string nickname)
    {
        Nick = nickname;
    }

    public void SetNick(string oldNick, string nickname)
    {
        _nicknameText.text = nickname;
    }
}
