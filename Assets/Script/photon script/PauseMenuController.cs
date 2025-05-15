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
        returnButton.onClick.AddListener(ReturnToSelectLevel);

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
                ReturnToSelectLevel();
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

    void ReturnToSelectLevel()
    {
        if (Runner != null)
        {
            Runner.LoadScene("LevelSelect");
        }
        // Replace with your select level scene name
        Debug.Log("Return to Select Level triggered"); // Optional debug log
    }
}