using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector3 WindDirection;
    void Awake()
    {
        WindDirection = new Vector3(Random.Range(-5, 5), .0f, .0f);
    }
}
