using Fusion;
using Fusion.Addons.Physics;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer[] _visuals;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private int maxHP = 100;

    [SerializeField]
    private AudioClip[] _audioClips;

    [SerializeField]
    private float _speed = 5f;

    [Networked, OnChangedRender(nameof(UpdateHealthBar))]
    public int Hp { get; set; }

    [Networked]
    private Angle _yaw { get; set; }
    [Networked]
    private Angle _pitch { get; set; }
    [Networked]
    private NetworkButtons _jumpPreviousButton { get; set; }
    [Networked]
    private NetworkButtons _Skill1PreviousButton { get; set; }
    [Networked]
    private NetworkButtons _Skill2PreviousButton { get; set; }
    [Networked]
    private NetworkButtons _Skill3PreviousButton { get; set; }
    [Networked]
    private NetworkButtons _VoiceChatPreviousButton { get; set; }

    private TimeStopAreaSpawner timeStopAreaSpawner;
    private Rigidbody rb;
    [Networked]
    public bool timeControlPlayer { get; set; } = false;
    ManipulateEnergy manipulateEnergy;
    [Networked]
    public bool manipulateEnergyPlayer { get; set; } = false;
    public HealthBar healthBar;

    private ChangeDetector _changeDetector;
    [SerializeField]
    GameObject energyUseageText;

    [SerializeField]
    Transform groundCheck;
    bool isGrounded = false;
    [SerializeField]
    float jumpHeight = 5f;
    [SerializeField]
    float gravity = 5f;
    bool manipulatingObject = false;
    [SerializeField] float invincibleTime = 1f;
    float invincibleTimer = 0f;
    [SerializeField] Sprite openMicSprite, offMicSprite;
    public Transform respawnPoint;
    Animator animator;

    // Added networked property for player name
    [Networked]
    public string PlayerName { get; set; }
    public Text playerNameText;
    [SerializeField] Image[] skillIcons;
    [SerializeField] Sprite[] skillSprites;
    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] Button restartButton;
    [SerializeField] Button returnButton;
    private bool isPaused = false;
    [SerializeField] GameObject loadingScreen;

    public override async void Spawned()
    {
        await Task.Delay(1500);
        Hp = maxHP;

        if (Object.HasInputAuthority)
        {
            _camera.enabled = true;
            _camera.GetComponent<AudioListener>().enabled = true;

            foreach (var visual in _visuals)
            {
                visual.enabled = false;
            }
            Camera.main.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Rpc_SetLayerInChildren();

            // Set the player name from the lobby settings
            RPC_SetPlayerName(PlayerSettings.PlayerName);
        }
        else
        {
            _camera.enabled = false;
            _camera.GetComponent<AudioListener>().enabled = false;

            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;
        }

        rb = GetComponent<Rigidbody>();

        timeStopAreaSpawner = GetComponent<TimeStopAreaSpawner>();
        manipulateEnergy = GetComponent<ManipulateEnergy>();
        BasicSpawner basicSpawner = FindObjectOfType<BasicSpawner>();
        IsVisible isVisible = GetComponentInChildren<IsVisible>();
        if (basicSpawner != null)
        {
            NetworkObject playerObject = GetComponent<NetworkObject>();
            Debug.Log(gameObject.name + ": " + playerObject.InputAuthority.ToString());
            if (playerObject.InputAuthority.ToString() == "[Player:2]")
            {
                timeControlPlayer = true;
            }
            else if (playerObject.InputAuthority.ToString() == "[Player:1]")
            {
                manipulateEnergyPlayer = true;
            }
            if (timeControlPlayer)
            {
                Destroy(manipulateEnergy);
                Rpc_DisableEnergyUsageText();
                timeStopAreaSpawner.enabled = true;
                Destroy(isVisible);
            }
            else if (manipulateEnergyPlayer)
            {
                Destroy(timeStopAreaSpawner);
                manipulateEnergy.enabled = true;
                energyUseageText.SetActive(true);
                energyUseageText.GetComponent<Text>().text = "Use Energy Amount: 0";
            }
            else
            {
                Destroy(timeStopAreaSpawner);
                Destroy(manipulateEnergy);
                Rpc_DisableEnergyUsageText();
            }
        }

        // Initialize the ChangeDetector
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        // Initialize the health bar
        UpdateHealthBar();

        FindObjectOfType<EnergyBank>().Rpc_UpdateStoredEnergy();

        animator = GetComponent<Animator>();

        UpdatePlayerNameUI();
        if (timeControlPlayer)
        {
            skillIcons[0].sprite = skillSprites[0];
            skillIcons[1].sprite = skillSprites[1];
            skillIcons[2].sprite = skillSprites[2];

        }
        else
        {
            skillIcons[0].sprite = skillSprites[3];
            skillIcons[1].sprite = skillSprites[4];
            skillIcons[2].sprite = skillSprites[5];
        }

        loadingScreen.SetActive(false);
    }

    public override void FixedUpdateNetwork()
    {
        if (!SceneManager.GetActiveScene().isLoaded)
        {
            return;
        }
        if (GetInput(out InputData data))
        {
            var jumpButtonPressed = data.JumpButton.GetPressed(_jumpPreviousButton);
            _jumpPreviousButton = data.JumpButton;

            var Skill1ButtonPressed = data.Skill1Button.GetPressed(_Skill1PreviousButton);
            _Skill1PreviousButton = data.Skill1Button;

            var Skill2ButtonPressed = data.Skill2Button.GetPressed(_Skill2PreviousButton);
            _Skill2PreviousButton = data.Skill2Button;

            var Skill3ButtonPressed = data.Skill3Button.GetPressed(_Skill3PreviousButton);
            _Skill3PreviousButton = data.Skill3Button;

            var VoiceChatButtonPressed = data.VoiceChatButton.GetPressed(_VoiceChatPreviousButton);
            _VoiceChatPreviousButton = data.VoiceChatButton;

            Vector3 moveInput = Vector3.zero;
            if (data.MoveInput.x > 0)
            {
                moveInput += Vector3.right;
            }
            if (data.MoveInput.x < 0)
            {
                moveInput += Vector3.left;
            }
            if (data.MoveInput.y > 0)
            {
                moveInput += Vector3.forward;
            }
            if (data.MoveInput.y < 0)
            {
                moveInput += Vector3.back;
            }
            //Vector3 tempRbVelocity = rb.velocity;
            if (moveInput != Vector3.zero && rb != null && isGrounded)
            {
                rb.AddForce(transform.rotation * moveInput * _speed, ForceMode.Force);
                animator.SetBool("isWalking", true);
            }
            else if (moveInput != Vector3.zero && rb != null && !isGrounded)
            {
                rb.AddForce(transform.rotation * moveInput * _speed / 20, ForceMode.Force);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
            if (isGrounded)
            {
                rb.drag = 10;
            }
            else
            {
                rb.drag = 0.5f;
                rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
            }
            if (!manipulatingObject)
            {
                HandlePitchYaw(data);
            }
            else
            {
                manipulateObject(data);
            }
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, LayerMask.GetMask("Default"));
            animator.SetBool("onGround", isGrounded);
            if (isGrounded && jumpButtonPressed.IsSet(InputButton.Jump))
            {
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                animator.SetTrigger("jump");
            }
            if (Skill1ButtonPressed.IsSet(InputButton.Skill1))
            {
                if (timeControlPlayer && timeStopAreaSpawner != null)
                {
                    timeStopAreaSpawner.SpawnObject();
                    Rpc_Playsound4();

                }
                if (manipulateEnergyPlayer && manipulateEnergy != null)
                {
                    manipulateEnergy.AbosrbEnergy();
                    Rpc_Playsound0();


                }
            }
            if (Skill2ButtonPressed.IsSet(InputButton.Skill2))
            {
                if (timeControlPlayer && timeStopAreaSpawner != null)
                {
                    timeStopAreaSpawner.RewindObject();
                    Rpc_Playsound3();

                }
                if (manipulateEnergyPlayer && manipulateEnergy != null)
                {
                    manipulateEnergy.KnockbackPlayer();
                    Rpc_Playsound1();

                }
            }
            if (Skill3ButtonPressed.IsSet(InputButton.Skill3))
            {
                if (timeControlPlayer && timeStopAreaSpawner != null)
                {
                    timeStopAreaSpawner.ActivateRecordAndRewindPlayer();

                }
                if (manipulateEnergyPlayer && manipulateEnergy != null && !manipulatingObject)
                {
                    manipulateEnergy.UseEnergy();
                }
            }
            SelectObject selectObject = GetComponent<SelectObject>();
            GameObject selectedObject = null;
            if (selectObject != null)
            {
                selectedObject = selectObject.selectedObject;
            }
            if (data.Skill3Button.IsSet(InputButton.Skill3) && manipulateEnergyPlayer && selectedObject != null)
            {
                manipulatingObject = true;
            }
            else
            {
                manipulatingObject = false;
            }
            if (data.ScrollInput != 0)
            {
                if (manipulateEnergyPlayer && manipulateEnergy != null)
                {
                    manipulateEnergy.SetEnergyUsage(data.ScrollInput);
                    energyUseageText.GetComponent<Text>().text = "Use Energy Amount: " + manipulateEnergy.useEnergyAmount;
                }
            }
            if (VoiceChatButtonPressed.IsSet(InputButton.VoiceChat))
            {
                Rpc_SetMicOnOff();
            }
        }

        transform.rotation = Quaternion.Euler(0, (float)_yaw, 0);
        _camera.transform.rotation = Quaternion.Euler((float)_pitch, (float)_yaw, 0);

        // Detect changes in Hp and update the health bar
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            if (change == nameof(Hp))
            {
                UpdateHealthBar();
            }
        }

        invincibleTimer -= Runner.DeltaTime;

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            if (change == nameof(PlayerName))
            {
                UpdatePlayerNameUI();
            }
        }

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Rpc_Respawn(Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (SceneManager.GetActiveScene().name == "Level1V2")
                Rpc_Respawn(new Vector3(15.828432083129883f, 3.513843536376953f, 63.08114242553711f));
            else if (SceneManager.GetActiveScene().name == "Level2")
            {
                if (timeControlPlayer)
                {
                    Rpc_Respawn(new Vector3(80.64547729492188f, -5.280000686645508f, -44.17205047607422f));
                }
                else if (manipulateEnergyPlayer)
                {
                    Rpc_Respawn(new Vector3(73.71570587158203f, -2.172490119934082f, -22.35167694091797f));
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (SceneManager.GetActiveScene().name == "Level1V2")
                Rpc_Respawn(new Vector3(15.860000610351563f, 0.7907352447509766f, 87.43000030517578f));
            else if (SceneManager.GetActiveScene().name == "Level2")
            {
                if (timeControlPlayer)
                {
                    Rpc_Respawn(new Vector3(80.99810791015625f, 8.55065631866455f, -14.512619018554688f));
                }
                else if (manipulateEnergyPlayer)
                {
                    Rpc_Respawn(new Vector3(88.91313934326172f, -3.3038883209228517f, -22.35167694091797f));
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (SceneManager.GetActiveScene().name == "Level1V2")
                Rpc_Respawn(new Vector3(18.84000015258789f, 3.770000457763672f, 149.9199981689453f));
            else if (SceneManager.GetActiveScene().name == "Level2")
                Rpc_Respawn(new Vector3(80.20999908447266f, 33.45000076293945f, 75.16000366210938f));

        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (SceneManager.GetActiveScene().name == "Level1V2")
                Rpc_Respawn(new Vector3(-34.90999984741211f, 0.4700002670288086f, 210.14999389648438f));
            else if (SceneManager.GetActiveScene().name == "Level2")
                Rpc_Respawn(new Vector3(93.71324920654297f, 38.39402389526367f, 190.70864868164063f));
        }
    }

    private void HandlePitchYaw(InputData data)
    {
        _yaw += data.Yaw;
        _pitch += data.Pitch;

        if (_pitch >= 180 && _pitch <= 270)
        {
            _pitch = 271;
        }
        else if (_pitch <= 180 && _pitch >= 90)
        {
            _pitch = 89;
        }
    }
    [Rpc]
    public void Rpc_Playsound0()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(_audioClips[0]);
        }
    }
    [Rpc]
    public void Rpc_Playsound1()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(_audioClips[1]);
        }
    }
    [Rpc]
    public void Rpc_Playsound2()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(_audioClips[2]);
        }
    }
    [Rpc]
    public void Rpc_Playsound3()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(_audioClips[3]);
        }
    }
    [Rpc]
    public void Rpc_Playsound4()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(_audioClips[4]);
        }
    }

    public void TakeDamage(int damage)
    {
        if (invincibleTimer >= 0)
            return;
        AudioSource audioSource = GetComponent<AudioSource>();

        Hp -= damage;
        audioSource.PlayOneShot(_audioClips[5]);

        invincibleTimer = invincibleTime;
        Debug.Log($"Player took {damage} damage. Current HP: {Hp}");
        if (Hp <= 0)
        {
            if (respawnPoint != null)
            {
                Rpc_Respawn(respawnPoint.position);
            }
            else
            {
                Rpc_Respawn(Vector3.zero);
            }
        }
    }

    [Rpc]
    public void Rpc_Respawn(Vector3 respawnPosition)
    {
        Hp = maxHP;
        UpdateHealthBar();
        NetworkRigidbody3D networkRigidbody3d = GetComponent<NetworkRigidbody3D>();
        networkRigidbody3d.Teleport(respawnPosition, Quaternion.identity);
        Debug.Log("Player respawned");
    }


    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(Hp, maxHP);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_SetLayerInChildren()
    {
        foreach (Transform trans in transform.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = LayerMask.NameToLayer("LocalPlayerModel");
        }
    }

    [Rpc]
    public void Rpc_DisableEnergyUsageText()
    {
        energyUseageText.SetActive(false);
    }

    void manipulateObject(InputData data)
    {
        GameObject selectedObject = GetComponent<SelectObject>().selectedObject;
        if (selectedObject != null && !selectedObject.CompareTag("Turret"))
        {
            selectedObject.transform.Rotate(Vector3.down, (float)data.Yaw);
            selectedObject.transform.Rotate(Vector3.right, (float)data.Pitch);

            if (selectedObject.GetComponent<TimeControl>() != null)
            {
                if (selectedObject.GetComponent<TimeControl>().timeStopped)
                {
                    selectedObject.GetComponent<TimeControl>().storedForce = selectedObject.transform.forward * selectedObject.GetComponent<TimeControl>().storedForce.magnitude;
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetMicOnOff()
    {
        Debug.Log("Rpc_SetMicOnOff called");
        Speaker speaker = GetComponent<Speaker>();
        speaker.enabled = !speaker.enabled;
        Image mic = transform.Find("Canvas").Find("mic").GetComponent<Image>();
        if (mic != null && openMicSprite != null && offMicSprite != null)
        {
            mic.sprite = speaker.enabled ? openMicSprite : offMicSprite;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestRespawn()
    {
        // Respawn logic, e.g.:
        transform.position = respawnPoint.position; // Ensure respawnPoint is defined
        Hp = maxHP; // Reset health or other states as needed
    }
    // Added RPC to set the player name
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetPlayerName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            PlayerName = name;
            UpdatePlayerNameUI();
        }
    }


    private void UpdatePlayerNameUI()
    {
        if (playerNameText != null)
        {
            playerNameText.text = PlayerName;
        }
    }

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