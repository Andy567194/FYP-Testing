using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class LevelChangeTrigger : NetworkBehaviour
{
    [Networked] public bool hasTriggered { get; set; } = false; // Networked to sync across clients

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
            // Get the current scene's build index
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

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
                Debug.Log($"Host triggered scene load from index {currentSceneIndex} to {nextSceneName} (index {nextSceneIndex})");
            }
            else
            {
                Debug.LogWarning("No next scene available in Build Settings!");
            }
        }
    }
}