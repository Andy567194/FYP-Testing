using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TimeControl : NetworkBehaviour
{
    [Networked] public bool timeStopped { get; set; }
    public LineRenderer lineRenderer;
    public float arrowLength = 2f;
    bool recovered = false;
    [Networked] private Vector3 tempVelocity { get; set; }
    [Networked] public Vector3 storedForce { get; set; }
    Rigidbody rb;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        if (lineRenderer == null)
        {
            var line = GetComponentInChildren<LineRenderer>();
            if (line != null)
            {
                lineRenderer = line;
            }
        }
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
        //takePowerButton.SetActive(false);
    }

    public override void FixedUpdateNetwork()
    {
        if (rb == null)
        {
            return;
        }
        if (timeStopped)
        {
            rb.isKinematic = true;
            if (recovered)
            {
                storedForce += tempVelocity * rb.mass;
            }
            recovered = false;
            if (lineRenderer != null)
                Rpc_ShowForceDirection();
            //takePowerButton.SetActive(true);
        }
        else
        {
            rb.isKinematic = false;
            if (!recovered)
            {
                rb.AddForce(storedForce, ForceMode.Impulse);
                storedForce = Vector3.zero;
                recovered = true;
            }
            tempVelocity = rb.velocity;
            if (lineRenderer != null)
                Rpc_hideLineRenderer();
            //takePowerButton.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb == null)
        {
            return;
        }
        if (timeStopped)
        {
            Vector3 force = collision.relativeVelocity * collision.rigidbody.mass;
            storedForce += force;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void Rpc_ShowForceDirection()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + storedForce.normalized * arrowLength);

        float forceMagnitude = storedForce.magnitude;
        Color arrowColor = Color.Lerp(Color.green, Color.red, forceMagnitude / 100f);
        lineRenderer.startColor = arrowColor;
        lineRenderer.endColor = arrowColor;
    }

    public void TakePower()
    {
        PowerBank powerBank = FindObjectOfType<PowerBank>();
        powerBank.AddPower(storedForce);
        storedForce = Vector3.zero;
    }

    public void SetTimeStopped(bool value)
    {
        timeStopped = value;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void Rpc_hideLineRenderer()
    {
        lineRenderer.enabled = false;
    }
}