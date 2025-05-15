using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;

public class PauseMenuController : NetworkBehaviour
{
    public GameObject pauseMenuPanel;
    public Button restartButton;
    public Button returnButton;
    private bool isPaused = false;

    public override void Spawned()
    {
        pauseMenuPanel.SetActive(false);
        //  restartButton.onClick.AddListener(RestartLevel);
        //_runner = FindObjectOfType<NetworkRunner>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseMenuPanel.SetActive(isPaused);
        }

        // Check for key presses when paused
        if (isPaused)
        {
            // if (Input.GetKeyDown(KeyCode.Alpha1))
            // {
            //    RestartLevel();
            // }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Rpc_ReturnToSelectLevel();
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    //void RestartLevel()
    //{

    //  {
    //    localPlayerController.Rpc_RespawnPlayer();
    //}
    //isPaused = false;
    //pauseMenuPanel.SetActive(false);
    //Debug.Log("Restart Level triggered"); // Optional debug log
    // }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ReturnToSelectLevel()
    {
        if (Runner != null)
        {
            Runner.LoadScene("LevelSelect");
        }
        // Replace with your select level scene name
        Debug.Log("Return to Select Level triggered"); // Optional debug log
    }
}