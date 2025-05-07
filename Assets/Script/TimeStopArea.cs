using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TimeStopArea : NetworkBehaviour
{
    private List<TimeControl> timeControllableObjects = new List<TimeControl>();
    private List<Antimatter> antimatterObjects = new List<Antimatter>();
    private List<MovingPlatform> movingPlatformObjects = new List<MovingPlatform>();
    private List<PressurePlate> pressurePlateObjects = new List<PressurePlate>();
    private List<FireTurret> fireTurretsObjects = new List<FireTurret>();
    private List<RollingBall> rollingBallObjects = new List<RollingBall>();


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
        if (other.CompareTag("MovingPlatform"))
        {
            MovingPlatform movingPlatform = other.GetComponent<MovingPlatform>();
            if (movingPlatform != null)
            {
                movingPlatform.SetTimeStopped(true);
            }
            if (!movingPlatformObjects.Contains(movingPlatform))
            {
                movingPlatformObjects.Add(movingPlatform);
            }
        }
        if (other.CompareTag("PressurePlate"))
        {
            PressurePlate pressurePlate = other.GetComponent<PressurePlate>();
            if (pressurePlate != null)
            {
                pressurePlate.SetTimeStopped(true);
            }
            if (!pressurePlateObjects.Contains(pressurePlate))
            {
                pressurePlateObjects.Add(pressurePlate);
            }
        }
        if (other.CompareTag("FireTurret"))
        {
            FireTurret fireTurret = other.GetComponentInChildren<FireTurret>();
            if (fireTurret != null)
            {
                fireTurret.RPC_SetTimeStop(true);
            }
            if (!fireTurretsObjects.Contains(fireTurret))
            {
                fireTurretsObjects.Add(fireTurret);
            }
        }
        if (other.CompareTag("RollingBall"))
        {
            RollingBall rollingBall = other.GetComponent<RollingBall>();
            if (rollingBall != null)
            {
                rollingBall.SetTimeStopped(true);
            }
            if (!rollingBallObjects.Contains(rollingBall))
            {
                rollingBallObjects.Add(rollingBall);
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
        if (other.CompareTag("MovingPlatform"))
        {
            MovingPlatform movingPlatform = other.GetComponent<MovingPlatform>();
            if (movingPlatform != null)
            {
                movingPlatform.SetTimeStopped(false);
            }
            movingPlatformObjects.Remove(movingPlatform);
        }
        if (other.CompareTag("PressurePlate"))
        {
            PressurePlate pressurePlate = other.GetComponent<PressurePlate>();
            if (pressurePlate != null)
            {
                pressurePlate.SetTimeStopped(false);
                pressurePlateObjects.Remove(pressurePlate);
            }
        }
        if (other.CompareTag("FireTurret"))
        {
            FireTurret fireTurret = other.GetComponentInChildren<FireTurret>();
            if (fireTurret != null)
            {
                fireTurret.RPC_SetTimeStop(false);
                fireTurretsObjects.Remove(fireTurret);
            }
        }
        if (other.CompareTag("RollingBall"))
        {
            RollingBall rollingBall = other.GetComponent<RollingBall>();
            if (rollingBall != null)
            {
                rollingBall.SetTimeStopped(false);
                rollingBallObjects.Remove(rollingBall);
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
        foreach (var movingPlatform in movingPlatformObjects)
        {
            movingPlatform.SetTimeStopped(false);
        }
        movingPlatformObjects.Clear();
        foreach (var pressurePlate in pressurePlateObjects)
        {
            pressurePlate.SetTimeStopped(false);
        }
        pressurePlateObjects.Clear();
        foreach (var fireTurret in fireTurretsObjects)
        {
            fireTurret.RPC_SetTimeStop(false);
        }
        fireTurretsObjects.Clear();
        foreach (var rollingBall in rollingBallObjects)
        {
            rollingBall.SetTimeStopped(false);
        }
        rollingBallObjects.Clear();
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
        foreach (var movingPlatform in movingPlatformObjects)
        {
            movingPlatform.SetTimeStopped(false);
        }
        movingPlatformObjects.Clear();
        foreach (var pressurePlate in pressurePlateObjects)
        {
            pressurePlate.SetTimeStopped(false);
        }
        pressurePlateObjects.Clear();
        foreach (var fireTurret in fireTurretsObjects)
        {
            fireTurret.RPC_SetTimeStop(false);
        }
        fireTurretsObjects.Clear();
        foreach (var rollingBall in rollingBallObjects)
        {
            rollingBall.SetTimeStopped(false);
        }
        rollingBallObjects.Clear();
    }
}