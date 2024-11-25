using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using UnityEngine.SceneManagement;

public class HubManager : MonoBehaviour
{
    private static HubManager _instance;
    public static HubManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<HubManager>();
            }
            return _instance;
        }
    }

    private List<string> _listNickname = new List<string>();
    private Dictionary<NetworkConnectionToClient, PlayerSetting> _dictPlayer = new Dictionary<NetworkConnectionToClient, PlayerSetting>();

    private List<Scene> _listScene = new List<Scene>();

    private void Start()
    {
        NetworkServer.RegisterHandler<PlayerMessageToServer>(OnPlayerMessageToServer);
    }

    private void OnPlayerMessageToServer(NetworkConnectionToClient conn, PlayerMessageToServer msg)
    {
        Debug.LogError("Send NickName");

        if (!_listNickname.Contains(msg.Nickname))
        {
            conn.Send(new ServerMessageToPlayer() {
                StateHub = StateHub.Nickname,
                Nickname = msg.Nickname
            });

            _listNickname.Add(msg.Nickname);
            _dictPlayer.Add(conn, new PlayerSetting() {Nickname = msg.Nickname });
            return;
        }

        conn.Send(new ServerMessageToPlayer()
        {
            StateHub = StateHub.Nickname,
            Nickname = ""
        });
    }

    public void AddVoidScene()
    {
        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        _listScene.Add(newScene);
        Debug.Log("[HubManager] Create void scene");
    }

    public void AddPlayerOnRandomScene(NetworkConnectionToClient conn)
    {
        Debug.LogError("Random");
        Scene scene = _listScene[Random.Range(0, _listScene.Count)];
        SceneManager.MoveGameObjectToScene(conn.identity.gameObject, scene);
        //Scene scene = player.gameObject.scene;

        Bomberman bomberman = conn.identity.gameObject.GetComponent<Bomberman>();
        Debug.LogError($"bomberman: {bomberman}");
        bomberman.GameParameters = GetGameParametersFromScene(scene);
        bomberman.SetBomberman();
    }

    public void AddPlayerOnScene(NetworkConnectionToClient conn, int scneneNumber)
    {
        Scene scene = _listScene[scneneNumber];
        SceneManager.MoveGameObjectToScene(conn.identity.gameObject, scene);
    }

    public void AddPlayerOnLastScene(NetworkConnectionToClient conn)
    {
        Scene scene = _listScene[_listScene.Count - 1];
        SceneManager.MoveGameObjectToScene(conn.identity.gameObject, scene);
    }

    public void SetGameParametersPlayer(Bomberman player)
    {
        Scene scene = player.gameObject.scene;
        player.GameParameters = GetGameParametersFromScene(scene);
        player.SetBomberman();
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
        Debug.LogError($"GetGameParametersFromScene: {null}");
        return null;
    }
}

public struct PlayerSetting
{
    public string Nickname;
}

public enum StateHub
{
    Nickname
}
