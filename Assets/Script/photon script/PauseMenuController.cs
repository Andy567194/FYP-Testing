using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public Button restartButton;
    public Button returnButton;
    private bool isPaused = false;
    private PlayerController localPlayerController;
    private NetworkRunner _runner;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
      //  restartButton.onClick.AddListener(RestartLevel);
        returnButton.onClick.AddListener(ReturnToSelectLevel);

        _runner = FindObjectOfType<NetworkRunner>();
        if (_runner != null)
        {
            NetworkObject playerObject = _runner.GetPlayerObject(_runner.LocalPlayer);
            if (playerObject != null)
            {
                localPlayerController = playerObject.GetComponent<PlayerController>();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseMenuPanel.SetActive(isPaused);
            if (localPlayerController != null)
            {
                localPlayerController.isPaused = isPaused;
            }
        }

        // Check for key presses when paused
        if (isPaused)
        {
           // if (Input.GetKeyDown(KeyCode.Alpha1))
           // {
            //    RestartLevel();
           // }
          if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                ReturnToSelectLevel();
            }
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
      if (_runner != null)
       {
            _runner.LoadScene("LevelSelect");
        }
 // Replace with your select level scene name
        Debug.Log("Return to Select Level triggered"); // Optional debug log
   }
}