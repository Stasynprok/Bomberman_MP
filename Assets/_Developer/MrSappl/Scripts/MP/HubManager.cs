using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

public class HubManager : MonoBehaviour
{
    private List<string> _listNickname = new List<string>();
    private Dictionary<NetworkConnectionToClient, PlayerSetting> _dictPlayer = new Dictionary<NetworkConnectionToClient, PlayerSetting>();

    private void Start()
    {
        NetworkServer.RegisterHandler<PlayerMessageToServer>(OnPlayerMessageToServer);
    }

    private void OnPlayerMessageToServer(NetworkConnectionToClient conn, PlayerMessageToServer msg)
    {
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
}

public struct PlayerSetting
{
    public string Nickname;
}

public enum StateHub
{
    Nickname
}
