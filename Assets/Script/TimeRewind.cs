using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TimeRewind : NetworkBehaviour
{
    private Rigidbody rb;
    private List<TransformData> transformData; // Store position and rotation
    private bool isRewinding = false;
    TimeControl timeControl;


    // Struct to hold position and rotation
    private struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformData(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        transformData = new List<TransformData>();
        timeControl = GetComponent<TimeControl>();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (isRewinding)
            {
                Rpc_Rewind();
            }
            else if (!timeControl.timeStopped)
            {
                Rpc_Record();
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_Rewind()
    {
        if (transformData.Count > 0)
        {
            // Get the last recorded transform data
            TransformData lastTransform = transformData[transformData.Count - 1];
            transform.position = lastTransform.position; // Move to the last recorded position
            transform.rotation = lastTransform.rotation; // Set the last recorded rotation
            transformData.RemoveAt(transformData.Count - 1); // Remove that data from the list
        }
        else
        {
            Rpc_setIsRewinding(false);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void Rpc_Record()
    {
        // Store both position and rotation
        transformData.Add(new TransformData(transform.position, transform.rotation));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_setIsRewinding(bool isRewinding)
    {
        this.isRewinding = isRewinding;
        rb.isKinematic = this.isRewinding;
    }
}