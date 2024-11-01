using Fusion;
using UnityEngine;

public class TimeStopAreaSpawner : NetworkBehaviour
{
    public GameObject objectToSpawn; // The prefab to instantiate
    public GameObject previewObject; // The prefab for the preview
    public float spawnDistance = 2f; // Distance from the player to spawn the object
    public float scrollSpeed = 1f;
    private GameObject currentPreview; // Reference to the current preview object
    private NetworkObject TSA;
    private bool spawned = false;
    Camera playerCamera;

    [Networked] private Vector3 previewPosition { get; set; }
    [Networked] private Quaternion previewRotation { get; set; }

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        //UpdatePreview();
    }

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
        }

        // Update networked variables
        previewPosition = spawnPosition;
        previewRotation = playerCamera.transform.rotation;
    }

    public void SetSpawnDistance(float scrollInput)
    {
        spawnDistance += scrollInput * scrollSpeed;
        spawnDistance = Mathf.Max(spawnDistance, 0);
    }

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
    /*
    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // Update the preview object position and rotation for all clients
            currentPreview.transform.position = previewPosition;
            currentPreview.transform.rotation = previewRotation;
        }
    }
    */
}