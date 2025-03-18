using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
