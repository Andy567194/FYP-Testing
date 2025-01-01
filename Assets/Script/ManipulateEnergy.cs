using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ManipulateEnergy : NetworkBehaviour
{

    GameObject selectedObject;
    public void AbosrbEnergy()
    {
        selectedObject = GetComponent<SelectObject>().selectedObject;
        if (selectedObject != null)
        {
            Vector3 storedForce = selectedObject.GetComponent<TimeControl>().storedForce;
            selectedObject.GetComponent<TimeControl>().storedForce = Vector3.zero;
            EnergyBank energyBank = FindObjectOfType<EnergyBank>().GetComponent<EnergyBank>();
            energyBank.AddEnergy(Mathf.Abs(storedForce.x) + Mathf.Abs(storedForce.y) + Mathf.Abs(storedForce.z));
        }
    }
}
