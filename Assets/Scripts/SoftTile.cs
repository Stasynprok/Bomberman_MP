using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftTile : NetworkBehaviour
{
    private Vector2Int cord;
    public ParticleSystem p;

    
    private void Start()
    {
        cord = GameParameters.Instance.PosToMat(transform.position);
    }
    private void OnEnable()
    {
        GameEvents.OnDestroyBomb += SelfDestroyHandler;
    }
    private void OnDisable()
    {
        GameEvents.OnDestroyBomb -= SelfDestroyHandler;
        GameParameters.Instance.SetElementToArray(cord.x, cord.y, 0);
    }
    private void SelfDestroyHandler(Vector2Int cord) {
        if (!GameParameters.Instance.SafeFromBomb(cord, this.cord)) {
            //Debug.Log("Tile Destroyed");
            GameEvents.OnDestroyTileInvoke();
            Instantiate(p, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }
}
