using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextHandler : MonoBehaviour
{
    public string input;

    public void Start()
    {

    }
    private void Update()
    {
        
    }

    public void ReadStringInput(string s)
    {
        input = s;
        Debug.Log(input);
    }
}