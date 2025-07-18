using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ManipulateEnergy : NetworkBehaviour
{
    public float useEnergyAmount = 0;
    public void AbosrbEnergy()
    {
        GameObject selectedObject = GetComponent<SelectObject>().selectedObject;
        if (selectedObject != null && selectedObject.CompareTag("TimeStoppable"))
        {
            Vector3 storedForce = selectedObject.GetComponentInParent<TimeControl>().storedForce;
            selectedObject.GetComponentInParent<TimeControl>().storedForce = Vector3.zero;
            EnergyBank energyBank = FindObjectOfType<EnergyBank>();
            energyBank.AddEnergy(Mathf.Abs(storedForce.x) + Mathf.Abs(storedForce.y) + Mathf.Abs(storedForce.z));
        }
    }

    public void KnockbackPlayer()
    {
        if (FindObjectOfType<EnergyBank>().storedEnergy >= useEnergyAmount)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject)
                {
                    TimeStopAreaSpawner timeStopAreaSpawner = player.GetComponent<TimeStopAreaSpawner>();
                    if (timeStopAreaSpawner != null)
                    {
                        timeStopAreaSpawner.AddStoredForce(useEnergyAmount);
                    }
                    FindObjectOfType<EnergyBank>().UseEnergy(useEnergyAmount);
                }
            }
        }
    }

    public void SetEnergyUsage(float scrollInput)
    {
        useEnergyAmount += scrollInput * 50;
        useEnergyAmount = Mathf.Max(useEnergyAmount, 0);
    }

    public void UseEnergy()
    {
        if (FindObjectOfType<EnergyBank>().storedEnergy >= useEnergyAmount)
        {
            GameObject selectedObject = GetComponent<SelectObject>().selectedObject;
            if (selectedObject != null && selectedObject.CompareTag("TimeStoppable"))
            {
                TimeControl timeControl = selectedObject.GetComponentInParent<TimeControl>();
                if (timeControl.timeStopped)
                {
                    timeControl.storedForce += Vector3.up * useEnergyAmount;
                }
                else
                {
                    selectedObject.GetComponent<Rigidbody>().AddForce(Vector3.up * useEnergyAmount, ForceMode.Impulse);
                }
                FindObjectOfType<EnergyBank>().UseEnergy(useEnergyAmount);
            }
        }
    }
}
