using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TimeControl : NetworkBehaviour
{
    [Networked] public bool timeStopped { get; set; }
    public LineRenderer lineRenderer;
    public GameObject takePowerButton;
    bool recovered = false;
    [Networked] private Vector3 tempVelocity { get; set; }
    [Networked] public Vector3 storedForce { get; set; }
    Rigidbody rb;

    private void Start()
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
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        //takePowerButton.SetActive(false);
    }

    void Update()
    {
        if (timeStopped)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
                if (recovered)
                {
                    storedForce += tempVelocity * rb.mass;
                }
                recovered = false;
                ShowForceDirection();
                //takePowerButton.SetActive(true);
            }
        }
        else
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                if (!recovered)
                {
                    rb.AddForce(storedForce, ForceMode.Impulse);
                    storedForce = Vector3.zero;
                    recovered = true;
                }
                tempVelocity = rb.velocity;
                lineRenderer.enabled = false;
                //takePowerButton.SetActive(false);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (timeStopped)
        {
            Vector3 force = collision.relativeVelocity * collision.rigidbody.mass;
            storedForce += force;
        }
    }

    void ShowForceDirection()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + storedForce.normalized * 2);

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
}