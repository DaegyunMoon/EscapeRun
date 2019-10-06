using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;

public class vCharacterState : MonoBehaviour
{
    public static vCharacterState instance;

    private int numOfGetItem;
    public int NumOfGetItem
    {
        get
        {
            return numOfGetItem;
        }
        set
        {
            numOfGetItem = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        numOfGetItem = 0;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
