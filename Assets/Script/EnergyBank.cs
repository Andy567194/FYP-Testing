using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class EnergyBank : NetworkBehaviour
{
    [Networked]
    public float storedEnergy { get; set; } = 0;
    public void AddEnergy(float energy)
    {
        storedEnergy += energy;
        Rpc_UpdateStoredEnergy();
    }

    public void UseEnergy(float energy)
    {
        storedEnergy -= energy;
        Rpc_UpdateStoredEnergy();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_UpdateStoredEnergy()
    {
        GameObject[] storedEnergyTexts = GameObject.FindGameObjectsWithTag("StoredEnergyText");
        foreach (GameObject storedEnergyText in storedEnergyTexts)
        {
            storedEnergyText.GetComponent<Text>().text = "Stored Energy: " + Mathf.Round(storedEnergy);
        }
    }
}
