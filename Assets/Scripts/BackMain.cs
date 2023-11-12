using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BackMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Back);
    }

    // Update is called once per frame
    void Back()
    {
        GameManager.SINGLETON.BackMain();
    }
}
