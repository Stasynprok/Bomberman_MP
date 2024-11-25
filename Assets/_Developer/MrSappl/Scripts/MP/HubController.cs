using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubController : MonoBehaviour
{
    [SerializeField] private InputNicknameController _inputNickNameController;
    public void Initialize()
    {
        gameObject.SetActive(true);
        NetworkClient.RegisterHandler<ServerMessageToPlayer>(OnServerMessageToPlayer);
    }

    public void SendNickname()
    {
        if (string.IsNullOrWhiteSpace(_inputNickNameController.InputNickname.text))
        {
            return;
        }

        NetworkClient.Send(new PlayerMessageToServer()
        {
            StateHub = StateHub.Nickname,
            Nickname = _inputNickNameController.InputNickname.text
        });
    }

    private void OnServerMessageToPlayer(ServerMessageToPlayer msg)
    {
        switch (msg.StateHub)
        {
            case StateHub.Nickname:
                CheckNickname(msg);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CheckNickname(ServerMessageToPlayer msg)
    {
        if (string.IsNullOrEmpty(msg.Nickname))
        {
            _inputNickNameController.ErrorNickname();
            return;
        }

        _inputNickNameController.gameObject.SetActive(false);

        NetworkClient.Send(new PlayerData()
        {
            Nickname = msg.Nickname
        });
    }
}
