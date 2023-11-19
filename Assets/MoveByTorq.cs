using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByTorq : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float _torque = 5;

    private Rigidbody _rigdbody;
    void Start()
    {
        _rigdbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxisRaw("Horizontal")!=0|| Input.GetAxisRaw("Vertical") != 0)
        {
            _rigdbody.AddTorque(new Vector3(Input.GetAxisRaw("Vertical")*_torque, 0, Input.GetAxisRaw("Horizontal") *_torque*-1));
        }
    }
}
