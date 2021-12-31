using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Address : MonoBehaviour
{

    int L, C = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void SetAddress(int L, int C)
    {
        L = L;
        C = C;
    }

    // Update is called once per frame
    public int[] GetAddress()
    {
        int[] address = { L, C };

        return (address);
    }
}
