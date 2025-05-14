using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private float _mouseSensitivity = 10f;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    public Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private List<PlayerRef> _playersToRespawn = new List<PlayerRef>();
    private NetworkRunner _runner;
    [SerializeField] Transform playerSpawnPoint;
    public bool changedScene = false;
    public int playerProgress = 0;

    void Start()
    {
        _runner = FindObjectOfType<NetworkRunner>();
        if (_runner != null)
        {
            _runner.AddCallbacks(this);
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var inputData = new InputData();
        if (Input.GetKey(KeyCode.W)) { inputData.MoveInput += Vector2.up; }
        if (Input.GetKey(KeyCode.S)) { inputData.MoveInput += Vector2.down; }
        if (Input.GetKey(KeyCode.A)) { inputData.MoveInput += Vector2.left; }
        if (Input.GetKey(KeyCode.D)) { inputData.MoveInput += Vector2.right; }

        inputData.Pitch = Input.GetAxis("Mouse Y") * _mouseSensitivity * (-1);
        inputData.Yaw = Input.GetAxis("Mouse X") * _mouseSensitivity;

        inputData.JumpButton.Set(InputButton.Jump, Input.GetKey(KeyCode.Space));
        inputData.Skill1Button.Set(InputButton.Skill1, Input.GetKey(KeyCode.Mouse1));
        inputData.ScrollInput = Input.GetAxis("Mouse ScrollWheel");
        inputData.PickUpButton.Set(InputButton.PickUp, Input.GetKey(KeyCode.Mouse0));
        inputData.Skill2Button.Set(InputButton.Skill2, Input.GetKey(KeyCode.E));
        inputData.Skill3Button.Set(InputButton.Skill3, Input.GetKey(KeyCode.Q));
        inputData.VoiceChatButton.Set(InputButton.VoiceChat, Input.GetKey(KeyCode.V));

        input.Set(inputData);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"OnPlayerJoined called for Player {player}, IsServer: {runner.IsServer}");
        SpawnPlayer(player);
    }

    private void SpawnPlayer(PlayerRef player)
    {
        if (_runner.IsServer)
        {
            NetworkObject networkPlayerObject;
            if (!changedScene)
            {
                Vector3 spawnPosition = playerSpawnPoint.position;
                networkPlayerObject = _runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            }
            else
            {
                networkPlayerObject = _runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);
            }

            networkPlayerObject.AssignInputAuthority(player);
            _spawnedCharacters[player] = networkPlayerObject;

            Debug.Log($"Player {player} spawned by host. Has InputAuthority: {networkPlayerObject.HasInputAuthority}");
        }
        else
        {
            Debug.Log($"Player {player} not spawned - client lacks authority.");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public async void OnSceneLoadDone(NetworkRunner runner)
    {
        if (runner.IsServer)
        {
            await Task.Delay(100);
            foreach (var player in _playersToRespawn)
            {
                SpawnPlayer(player);
                Debug.Log($"Scene loaded - Respawned Player {player} at {playerSpawnPoint.position}");
            }
            _playersToRespawn.Clear();
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        if (runner.IsServer)
        {
            _playersToRespawn.Clear();
            foreach (var player in _spawnedCharacters)
            {
                if (player.Value != null)
                {
                    _playersToRespawn.Add(player.Key);
                    runner.Despawn(player.Value);
                    Debug.Log($"Despawning Player {player.Key} before scene change.");
                }
            }
            _spawnedCharacters.Clear();
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}