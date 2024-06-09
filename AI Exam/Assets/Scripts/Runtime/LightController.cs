using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightController : MonoBehaviour
{

    public Light lightSource;

    void OnPreRender()
    {
        lightSource.enabled = false;      
    }

    void OnPostRender()
    {
        lightSource.enabled = true;
    }
}