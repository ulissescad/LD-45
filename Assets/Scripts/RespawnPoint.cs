using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField]
    GameObject _prefabEnemy;

    [SerializeField]
    Transform _respawn;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("RespawnEnemy", 1, 2);
    }

    // Update is called once per frame
    public void RespawnEnemy()
    {
        Instantiate(_prefabEnemy, _respawn.position, _respawn.rotation);
    }
}
