using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.CharacterSelection;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private int _instances = 3;
    [Scene, SerializeField] private string _gameScene;


    private static CustomNetworkManager _instance;
    public static CustomNetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CustomNetworkManager>();
            }
            return _instance;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<PlayerData>(OnCreateCharacter);
        StartCoroutine(ServerLoadSubScenes());
        
    }

    public override void OnStopClient()
    {
        StartCoroutine(ClientUnloadSubScenes());
    }

    private void OnCreateCharacter(NetworkConnectionToClient conn, PlayerData message)
    {
        GameObject gameobject = Instantiate(playerPrefab);

        Bomberman player = gameobject.GetComponent<Bomberman>();
        player.SetNickname(message.Nickname);

        NetworkServer.AddPlayerForConnection(conn, gameobject);
        HubManager.Instance.AddPlayerOnRandomScene(conn);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        conn.Send(new SceneMessage { sceneName = _gameScene, sceneOperation = SceneOperation.LoadAdditive });
    }

    [ServerCallback]
    private IEnumerator ServerLoadSubScenes()
    {
        for (int index = 1; index < _instances; index++)
        {
            yield return SceneManager.LoadSceneAsync(_gameScene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D});

            HubManager.Instance.AddVoidScene();
        }
    }


    private IEnumerator ClientUnloadSubScenes()
    {
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
    }
    

}
