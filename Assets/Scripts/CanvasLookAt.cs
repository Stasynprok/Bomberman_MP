using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAt : NetworkBehaviour
{
    private void Update()
    {
        if (isServer)
        {
            return;
        }
        LookAtCamera();
    }

    [Client]
    private void LookAtCamera()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
