using Fusion;
using Fusion.Addons.Physics;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private MeshRenderer[] _visuals;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private int maxHP = 100;
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
    bool manipulatingObject = false;
    [SerializeField] float invincibleTime = 1f;
    float invincibleTimer = 0f;
    [SerializeField] Sprite openMicSprite, offMicSprite;

    public override void Spawned()
    {
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
            int i = 0;
            foreach (var kvp in basicSpawner._spawnedCharacters)
            {
                if (kvp.Value == Object)
                {
                    break;
                }
                i++;
            }
            if (i == 1 && timeStopAreaSpawner != null && manipulateEnergy != null)
            {
                timeControlPlayer = true;
            }
            else if (i == 0 && manipulateEnergy != null && timeStopAreaSpawner != null)
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
    }

    public override void FixedUpdateNetwork()
    {
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
            Vector3 tempRbVelocity = rb.velocity;
            if (moveInput != Vector3.zero && rb != null && isGrounded)
            {
                //rb.velocity = transform.rotation * moveInput * _speed;
                //rb.velocity = new Vector3(rb.velocity.x, tempRbVelocity.y, rb.velocity.z);
                //if (rb.velocity.magnitude <= _speed)
                rb.AddForce(transform.rotation * moveInput * _speed, ForceMode.Force);
            }
            else if (moveInput != Vector3.zero && rb != null && !isGrounded)
            {
                //rb.velocity = transform.rotation * moveInput * _speed;
                //rb.velocity = new Vector3(rb.velocity.x, tempRbVelocity.y, rb.velocity.z);
                rb.AddForce(transform.rotation * moveInput * _speed / 20, ForceMode.Force);
            }
            if (isGrounded)
            {
                rb.drag = 10;
            }
            else
            {
                rb.drag = 0.5f;
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
            if (isGrounded && jumpButtonPressed.IsSet(InputButton.Jump))
            {
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            }
            if (Skill1ButtonPressed.IsSet(InputButton.Skill1))
            {
                if (timeControlPlayer && timeStopAreaSpawner != null)
                {
                    timeStopAreaSpawner.SpawnObject();
                }
                if (manipulateEnergyPlayer && manipulateEnergy != null)
                {
                    manipulateEnergy.AbosrbEnergy();
                }
            }
            if (Skill2ButtonPressed.IsSet(InputButton.Skill2))
            {
                if (timeControlPlayer && timeStopAreaSpawner != null)
                {
                    timeStopAreaSpawner.RewindObject();
                }
                if (manipulateEnergyPlayer && manipulateEnergy != null)
                {
                    manipulateEnergy.KnockbackPlayer();
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
            if (data.Skill3Button.IsSet(InputButton.Skill3) && manipulateEnergyPlayer)
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

    public void TakeDamage(int damage)
    {
        if (invincibleTimer >= 0)
            return;

        Hp -= damage;
        invincibleTimer = invincibleTime;
        Debug.Log($"Player took {damage} damage. Current HP: {Hp}");
    }
    /*  if (Hp <= 0)
      {
          // Define spawn points
          Vector3 defaultSpawn = new Vector3(17.73f, 7.094f, 7.81f);
          Vector3 phase1Spawn = new Vector3(33.36f, 8.990735f, 94.4f);// Adjust these coordinates
          Vector3 phase2Spawn = new Vector3(33.36f, 8.990735f, 94.4f);// Adjust these coordinates
          Vector3 phase3Spawn = new Vector3(33.36f, 8.990735f, 94.4f);    // Adjust these coordinates
          Vector3 phase4Spawn = new Vector3(36.34f, 11.97f, 156.89f);  // Adjust these coordinates
          Vector3 phase5Spawn = new Vector3(-13.5f, 9.29f, 216f);  // Adjust these coordinates

          // Find phase region GameObjects

          GameObject phase1Region = GameObject.Find("ShooterActivation 1");
          GameObject phase2Region = GameObject.Find("ShooterActivation 2");
          GameObject phase3Region = GameObject.Find("ShooterActivation 3");
          GameObject phase4Region = GameObject.Find("ShooterActivation 4");
          GameObject phase5Region = GameObject.Find("ShooterActivation 5");

          // Check each region and spawn accordingly
          if (phase5Region != null && phase5Region.GetComponent<Collider>()?.bounds.Contains(transform.position) == true)
          {
              Rpc_Respawn(phase5Spawn);
          }
          else if (phase4Region != null && phase4Region.GetComponent<Collider>()?.bounds.Contains(transform.position) == true)
          {
              Rpc_Respawn(phase4Spawn);
          }
          else if (phase3Region != null && phase3Region.GetComponent<Collider>()?.bounds.Contains(transform.position) == true)
          {
              Rpc_Respawn(phase3Spawn);
          }
          else if (phase2Region != null && phase2Region.GetComponent<Collider>()?.bounds.Contains(transform.position) == true)
          {
              Rpc_Respawn(phase2Spawn);
          }
          else if (phase1Region != null && phase1Region.GetComponent<Collider>()?.bounds.Contains(transform.position) == true)
          {
              Rpc_Respawn(phase1Spawn);
          }
          else
          {
              // Fallback to default spawn if not in any specific region
              Rpc_Respawn(defaultSpawn);
          }
      }
  } */

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_Respawn(Vector3 respawnPosition)
    {
        // Implement respawn logic here
        Hp = maxHP;
        UpdateHealthBar();
        // Move player to respawn position
        NetworkRigidbody3D networkRigidbody3d = GetComponent<NetworkRigidbody3D>();
        networkRigidbody3d.Teleport(respawnPosition, Quaternion.identity);
        //transform.position = new Vector3(3, 5, 0);
        // Example respawn position
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
        if (selectedObject != null)
        {
            selectedObject.transform.Rotate(Vector3.down, (float)data.Yaw);
            selectedObject.transform.Rotate(Vector3.right, (float)data.Pitch);

            if (selectedObject.GetComponent<TimeControl>().timeStopped)
            {
                selectedObject.GetComponent<TimeControl>().storedForce = selectedObject.transform.forward * selectedObject.GetComponent<TimeControl>().storedForce.magnitude;
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
}