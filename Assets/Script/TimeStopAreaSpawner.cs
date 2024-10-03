using UnityEngine;

public class TimeStopAreaSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // The prefab to instantiate
    public GameObject previewObject; // The prefab for the preview
    public float spawnDistance = 2f; // Distance from the player to spawn the object
    public float scrollSpeed = 1f;
    private GameObject currentPreview; // Reference to the current preview object
    GameObject TSA;
    private bool spawned = false;

    void Update()
    {
        // Check for mouse wheel input to adjust spawn distance
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            spawnDistance += scrollInput * scrollSpeed;
            spawnDistance = Mathf.Max(spawnDistance, 0.1f); // Prevent negative or zero distance
        }

        // Update the position of the preview object
        UpdatePreview();

        // Check for input to spawn the object
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SpawnObject();
            spawned = !spawned;
        }
    }

    void UpdatePreview()
    {
        // Get the player's camera
        Camera playerCamera = GetComponentInChildren<Camera>();

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
    }

    void SpawnObject()
    {
        if (!spawned)
        {
            // Instantiate the object at the calculated position with the same rotation as the camera
            if (currentPreview != null)
            {
                TSA = Instantiate(objectToSpawn, currentPreview.transform.position, currentPreview.transform.rotation);
                Destroy(currentPreview); // Remove the preview object after spawning
            }
        }
        else
        {
            Destroy(TSA);
        }

    }
}