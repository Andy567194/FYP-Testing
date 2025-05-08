using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class LevelChangeTrigger : NetworkBehaviour
{
    [Networked] public bool hasTriggered { get; set; } = false; // Networked to sync across clients

    [SerializeField] private int targetSceneIndex = -1; // Set the target scene index in the Inspector (-1 means no target set)

    public override void Spawned()
    {
        // Ensure the trigger is only active for the host

        Debug.Log(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "LevelSelect")
        {
            Debug.Log("LevelSelect scene loaded - Setting trigger active");
            BasicSpawner basicSpawner = FindObjectOfType<BasicSpawner>();
            if (basicSpawner != null)
            {
                if (basicSpawner.playerProgress + 1 >= targetSceneIndex)
                {
                    Debug.Log($"Player progress {basicSpawner.playerProgress} allows scene change to index {targetSceneIndex}");
                    Rpc_SetActive(true);
                }
                else
                {
                    Rpc_SetActive(false);
                    Debug.Log($"Player progress {basicSpawner.playerProgress} does not allow scene change to index {targetSceneIndex}");
                }
            }
        }

    }

    // Called when another collider enters this trigger
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter called - HasStateAuthority: {HasStateAuthority}, Other: {other.gameObject.name}");
        if (HasStateAuthority && !hasTriggered) // Only the host triggers the scene change
        {
            // Check if the object that entered is a player
            NetworkObject playerObject = other.GetComponentInParent<NetworkObject>();
            if (playerObject != null)
            {
                Debug.Log($"NetworkObject found - InputAuthority: {playerObject.InputAuthority}, PlayerObject: {Runner.GetPlayerObject(playerObject.InputAuthority)}");
                // Check if this NetworkObject is associated with a PlayerRef
                if (Runner.GetPlayerObject(playerObject.InputAuthority) != null)
                {
                    Debug.Log($"Player {playerObject.InputAuthority} entered the trigger!");
                    hasTriggered = true; // Mark as triggered (synced across network)
                    ChangeLevel();
                }
                // Fallback: Check if the object has a specific component or tag to identify it as a player
                else if (other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)
                {
                    Debug.Log($"Fallback: Identified {other.gameObject.name} as a player via tag/component!");
                    hasTriggered = true;
                    ChangeLevel();
                }
                else
                {
                    Debug.Log($"Object {other.gameObject.name} entered but is not recognized as a player.");
                }
            }
            else
            {
                Debug.Log($"Object {other.gameObject.name} entered but has no NetworkObject.");
            }
        }
    }

    private void ChangeLevel()
    {
        if (HasStateAuthority) // Ensure only the host initiates the scene load
        {
            int nextSceneIndex = targetSceneIndex >= 0 ? targetSceneIndex : SceneManager.GetActiveScene().buildIndex + 1;

            // Check if the next scene index is valid
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                string nextSceneName = SceneManager.GetSceneByBuildIndex(nextSceneIndex).name;
                if (string.IsNullOrEmpty(nextSceneName))
                {
                    // If the scene isn't loaded yet, use the path to extract the name
                    string scenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
                    nextSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                }

                // Load the next scene for all clients
                Debug.Log($"Loading scene: {nextSceneName} (index: {nextSceneIndex})");
                Runner.LoadScene(nextSceneName);
                BasicSpawner basicSpawner = FindObjectOfType<BasicSpawner>();
                if (basicSpawner != null)
                {
                    basicSpawner.changedScene = true; // Notify the spawner about the scene load
                }
                Debug.Log($"Host triggered scene load to {nextSceneName} (index {nextSceneIndex})");
                if (SceneManager.GetActiveScene().name == "Level1" && basicSpawner.playerProgress <= 0)
                    basicSpawner.playerProgress += 1; // Update player progress
                else if (SceneManager.GetActiveScene().name == "Level2" && basicSpawner.playerProgress <= 1)
                    basicSpawner.playerProgress += 1; // Update player progress
                else if (SceneManager.GetActiveScene().name == "Level3" && basicSpawner.playerProgress <= 2)
                    basicSpawner.playerProgress += 1; // Update player progress
                else if (SceneManager.GetActiveScene().name == "Level4 Tentative" && basicSpawner.playerProgress <= 3)
                    basicSpawner.playerProgress += 1; // Update player progress
            }
            else
            {
                Debug.LogWarning("No valid scene available in Build Settings for the specified index!");
            }
        }
    }
    [Rpc]
    void Rpc_SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}