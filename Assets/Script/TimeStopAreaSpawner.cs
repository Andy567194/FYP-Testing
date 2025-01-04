using Fusion;
using UnityEngine;

public class TimeStopAreaSpawner : NetworkBehaviour
{
    public GameObject objectToSpawn; // The prefab to instantiate
    //public GameObject previewObject; // The prefab for the preview
    //public float spawnDistance = 2f; // Distance from the player to spawn the object
    //public float scrollSpeed = 1f;
    //private GameObject currentPreview; // Reference to the current preview object
    private NetworkObject TSA;
    [Networked] public NetworkBool spawned { get; set; } = false;
    //[Networked] private Vector3 previewPosition { get; set; }
    //[Networked] private Quaternion previewRotation { get; set; }
    //[Networked] private Vector3 spawnedPosition { get; set; }
    //[Networked] private Quaternion spawnedRotation { get; set; }
    [Networked]
    public float storedForce { get; set; } = 0;

    public override void FixedUpdateNetwork()
    {
        if (spawned && TSA != null)
        {
            TSA.transform.position = transform.position;
        }
        if (storedForce > 0 && !spawned)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(GetComponentInChildren<Camera>().transform.forward * storedForce, ForceMode.Impulse);
            storedForce = 0;
        }
    }

    /*
    void UpdatePreview()
    {
        // Calculate the spawn position in front of the camera
        Vector3 spawnPosition = playerCamera.transform.position + playerCamera.transform.forward * spawnDistance;

        // If the preview object doesn't exist, create it
        if (currentPreview == null)
        {
            currentPreview = Instantiate(previewObject, spawnPosition, playerCamera.transform.rotation);
        }
        else
        {
            // Update the position and rotation of the preview object
            currentPreview.transform.position = spawnPosition;
            currentPreview.transform.rotation = playerCamera.transform.rotation;
        }

        // Update networked variables
        previewPosition = spawnPosition;
        previewRotation = playerCamera.transform.rotation;
    }*/

    /*
    public void SetSpawnDistance(float scrollInput)
    {
        spawnDistance += scrollInput * scrollSpeed;
        spawnDistance = Mathf.Max(spawnDistance, 0);
    }*/

    public void SpawnObject()
    {
        if (!spawned)
        {
            TSA = Runner.Spawn(objectToSpawn, transform.position, Quaternion.identity, Object.InputAuthority);
            spawned = true;

        }
        else
        {
            if (TSA != null)
            {
                Runner.Despawn(TSA);
                spawned = false;
            }
        }
    }

    public void AddStoredForce(float force)
    {
        storedForce += force;
    }

    public void RewindObject()
    {
        GameObject selectedObject = GetComponent<SelectObject>().selectedObject;
        if (selectedObject != null)
        {
            selectedObject.GetComponent<TimeRewind>().setIsRewinding(true);
        }
    }
}