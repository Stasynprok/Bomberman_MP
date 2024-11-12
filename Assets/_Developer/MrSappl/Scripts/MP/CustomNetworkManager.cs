using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.CharacterSelection;

public class CustomNetworkManager : NetworkManager
{
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
        
    }

    private void OnCreateCharacter(NetworkConnectionToClient conn, PlayerData message)
    {
        GameObject gameobject = Instantiate(playerPrefab);

        Bomberman player = gameobject.GetComponent<Bomberman>();
        player.SetNickname(message.Nickname);
        //player.SetBomberman(Grid.instance);
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }

    public override void OnClientConnect()
    {
        NetworkClient.Ready();
    }


}
