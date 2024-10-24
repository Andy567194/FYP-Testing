using Fusion;
using UnityEngine;

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
    private float _speed = 5f;
    [Networked]
    private Angle _yaw { get; set; }
    [Networked]
    private Angle _pitch { get; set; }
    [Networked]
    private NetworkButtons _previousButton { get; set; }

    public override void Spawned()
    {
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
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData data))
        {
            var buttonPressed = data.Button.GetPressed(_previousButton);
            _previousButton = data.Button;

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

            if (buttonPressed.IsSet(InputButton.Jump))
            {
                _characterController.Jump();
            }
        }

        transform.rotation = Quaternion.Euler(0, (float)_yaw, 0);

        //var cameraEulerAngle = _camera.transform.localRotation.eulerAngles;
        _camera.transform.rotation = Quaternion.Euler((float)_pitch, (float)_yaw, 0);
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
    public static void SetRenderLayerInChildren(Transform transform, int layerNumber) 
    {
        foreach (Transform trans in transform.GetComponentsInChildren<Transform>(true))
            trans.gameObject.layer = layerNumber;
    }
}