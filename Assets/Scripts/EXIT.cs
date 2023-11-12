using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EXIT : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(Exit);
    }

    // Update is called once per frame
    void Exit()
    {
        Application.Quit();
    }
}
