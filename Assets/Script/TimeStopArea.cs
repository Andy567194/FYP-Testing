using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TimeStopArea : NetworkBehaviour
{
    private List<TimeControl> timeControllableObjects = new List<TimeControl>();
    private List<Antimatter> antimatterObjects = new List<Antimatter>();


    void Start()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material = new Material(renderer.material);
            renderer.material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TimeStoppable"))
        {
            TimeControl timeControl = other.GetComponentInParent<TimeControl>();
            if (timeControl == null)
            {
                timeControl = other.GetComponent<TimeControl>();
            }
            if (!timeControllableObjects.Contains(timeControl))
            {
                timeControllableObjects.Add(timeControl);
            }
            timeControl.SetTimeStopped(true);
        }
        if (other.CompareTag("Antimatter"))
        {
            Antimatter antimatter = other.GetComponent<Antimatter>();
            if (antimatter != null)
            {
                antimatter.SetTimeStopped(true);
            }
            if (!antimatterObjects.Contains(antimatter))
            {
                antimatterObjects.Add(antimatter);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TimeStoppable"))
        {
            TimeControl timeControl = other.GetComponentInParent<TimeControl>();
            timeControl.SetTimeStopped(false);
            timeControllableObjects.Remove(timeControl);
            EnableGravity enableGravity = other.GetComponent<EnableGravity>();
            if (enableGravity != null)
            {
                enableGravity.Enable();
            }
        }
        if (other.CompareTag("Antimatter"))
        {
            Antimatter antimatter = other.GetComponent<Antimatter>();
            if (antimatter != null)
            {
                antimatter.SetTimeStopped(false);
                antimatterObjects.Remove(antimatter);
            }
        }
    }

    private void OnDisable()
    {
        foreach (var timeControl in timeControllableObjects)
        {
            timeControl.SetTimeStopped(false);
        }
        timeControllableObjects.Clear();
        foreach (var antimatter in antimatterObjects)
        {
            antimatter.SetTimeStopped(false);
        }
        antimatterObjects.Clear();
    }

    private void OnDestroy()
    {
        foreach (var timeControl in timeControllableObjects)
        {
            timeControl.SetTimeStopped(false);
        }
        timeControllableObjects.Clear();
        foreach (var antimatter in antimatterObjects)
        {
            antimatter.SetTimeStopped(false);
        }
        antimatterObjects.Clear();
    }
}