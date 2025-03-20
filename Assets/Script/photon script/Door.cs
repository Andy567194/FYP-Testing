using Fusion;
using UnityEngine;

public class Door : NetworkBehaviour
{
    private bool isUnlocked = false;

    public void Unlock()
    {
        if (!isUnlocked)
        {
            isUnlocked = true;
            // Add your door opening logic here
            Debug.Log("Door unlocked!");
            // For example, you can disable the collider to simulate the door being opened
            GetComponent<Collider>().enabled = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }
    }
}