using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    public Transform playerModel;
    [SerializeField]
    private NetworkCharacterController _characterController;
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

    private TimeStopAreaSpawner timeStopAreaSpawner;
    private Rigidbody _rigidbody;
    public bool timeControlPlayer = false;
    ManipulateEnergy manipulateEnergy;
    public bool manipulateEnergyPlayer = false;
    public HealthBar healthBar;

    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
            Hp = maxHP;

        if (Object.HasInputAuthority)
        {
            _camera.enabled = true;
            _camera.GetComponent<AudioListener>().enabled = true;

            foreach (var visual in _visuals)
            {
                visual.enabled = true;
            }
            SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));
            Camera.main.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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

        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
        }
        _rigidbody.useGravity = true;

        timeStopAreaSpawner = GetComponent<TimeStopAreaSpawner>();
        manipulateEnergy = GetComponent<ManipulateEnergy>();
        BasicSpawner basicSpawner = FindObjectOfType<BasicSpawner>();
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
                timeStopAreaSpawner.enabled = true;
                timeControlPlayer = true;
                manipulateEnergyPlayer = false;
                manipulateEnergy.enabled = false;
            }
            else if (i == 0 && manipulateEnergy != null && timeStopAreaSpawner != null)
            {
                timeControlPlayer = false;
                timeStopAreaSpawner.enabled = false;
                manipulateEnergyPlayer = true;
                manipulateEnergy.enabled = true;
            }
            else
            {
                timeControlPlayer = false;
                timeStopAreaSpawner.enabled = false;
                manipulateEnergyPlayer = false;
                manipulateEnergy.enabled = false;
            }
        }

        // Initialize the ChangeDetector
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        // Initialize the health bar
        UpdateHealthBar();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData data))
        {
            var jumpButtonPressed = data.JumpButton.GetPressed(_jumpPreviousButton);
            _jumpPreviousButton = data.JumpButton;

            var Skill1ButtonPressed = data.Skill1Button.GetPressed(_Skill1PreviousButton);
            _Skill1PreviousButton = data.Skill1Button;

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
            _characterController.Move(transform.rotation * moveInput * _speed * Runner.DeltaTime);

            HandlePitchYaw(data);

            if (jumpButtonPressed.IsSet(InputButton.Jump))
            {
                _characterController.Jump();
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
        if (Object.HasStateAuthority)
        {
            Hp -= damage;
            Debug.Log($"Player took {damage} damage. Current HP: {Hp}");
            if (Hp <= 0)
            {
                Respawn();
            }
        }
    }

    private void Respawn()
    {
        // Implement respawn logic here
        Hp = maxHP;
        UpdateHealthBar();
        // Move player to respawn position
        transform.position = new Vector3(3, 5, 0);
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

    public static void SetRenderLayerInChildren(Transform transform, int layerNumber)
    {
        foreach (Transform trans in transform.GetComponentsInChildren<Transform>(true))
            trans.gameObject.layer = layerNumber;
    }
}