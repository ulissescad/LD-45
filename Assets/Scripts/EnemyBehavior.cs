﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public int dir = 1;
    int mov = 0;

    public bool _isRespawnPoint = false;

    [SerializeField]
    public float _speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("MovObj", 1, _speed);
    }

    private void MovObj()
    {
        transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
        transform.Translate(1 * dir, 0, 0);
        mov++;
        if (mov > 16 && !_isRespawnPoint)
        {
            mov = 0;
            this.gameObject.SetActive(false);
            this.transform.position = new Vector3(0, 100, 0);
        }

    }

    public void ChangeDir()
    {
        dir *= -1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            GameManager.SINGLETON.Dead();
            this.gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Bonus")
        {
            collision.gameObject.SetActive(false);
            collision.gameObject.transform.position = new Vector3(0, 100, 0);
        }
    }
}
