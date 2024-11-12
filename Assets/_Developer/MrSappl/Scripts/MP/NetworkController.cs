using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviour
{
    public void ConnectosHostServer()
    {
        NetworkManager.singleton.StartHost();
    }

    public void ConnectClient()
    {
        NetworkManager.singleton.StartClient();
    }
}
