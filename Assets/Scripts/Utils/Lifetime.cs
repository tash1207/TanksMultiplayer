using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
