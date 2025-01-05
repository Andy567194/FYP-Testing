using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLookRotation : MonoBehaviour
{
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
    }
    void FixedUpdate()
    {
        Quaternion rotation = Quaternion.LookRotation(rb.velocity);
        rb.MoveRotation(rotation);
    }
}
