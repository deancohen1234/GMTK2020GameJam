using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float m_EnergyLost = 10.0f;

    public void DestoryBreakable () 
    {
        Destroy(gameObject);
    }
}
