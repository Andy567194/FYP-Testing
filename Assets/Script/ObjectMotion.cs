using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ObjectMotion : NetworkBehaviour
{
    public float rotationSpeed = 50f;
    TimeControl timeControl;

    public override void Spawned()
    {
        timeControl = GetComponent<TimeControl>();
    }

    public override void FixedUpdateNetwork()
    {
        //if (!timeControl.timeStopped)
        //{
        // Rotate the object around its Y axis at the specified speed
        transform.Rotate(Vector3.up, rotationSpeed * Runner.DeltaTime);
        //}
    }
}
