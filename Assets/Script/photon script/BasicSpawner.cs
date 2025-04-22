using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
//using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Fusion.Addons.Physics;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private float _mouseSensitivity = 10f;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    public Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private List<PlayerRef> _playersToRespawn = new List<PlayerRef>(); // Track players to respawn after scene change

    private NetworkRunner _runner;
    [SerializeField] Transform playerSpawnPoint;

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

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

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"OnPlayerJoined called for Player {player}, IsServer: {runner.IsServer}");
        SpawnPlayer(player);
    }
    private void SpawnPlayer(PlayerRef player)
    {
        // Only the host (server) should spawn NetworkObjects
        if (_runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = playerSpawnPoint.position;
            NetworkObject networkPlayerObject = _runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);

            // Assign input authority to the player object
            networkPlayerObject.AssignInputAuthority(player);

            // Keep track of the player avatars for easy access
            _spawnedCharacters[player] = networkPlayerObject;

            // Log whether the player has InputAuthority
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

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        // Implementation needed
    }

    public async void OnSceneLoadDone(NetworkRunner runner)
    {
        // After the scene loads, respawn players (only on the host)
        if (runner.IsServer)
        {
            // Delay to ensure despawn and spawn don't happen in the same tick
            await Task.Delay(100); // Wait 100ms to avoid same-tick spawn/despawn

            foreach (var player in _playersToRespawn)
            {
                // Respawn the player in the new scene
                SpawnPlayer(player);
                Debug.Log($"Scene loaded - Respawned Player {player} at {playerSpawnPoint.position}");
            }
            _playersToRespawn.Clear(); // Clear the list after respawning
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        // Before the scene unloads, despawn all players and store their PlayerRefs
        if (runner.IsServer)
        {
            _playersToRespawn.Clear();
            foreach (var player in _spawnedCharacters)
            {
                if (player.Value != null)
                {
                    _playersToRespawn.Add(player.Key); // Store PlayerRef for respawning
                    runner.Despawn(player.Value);
                    Debug.Log($"Despawning Player {player.Key} before scene change.");
                }
            }
            _spawnedCharacters.Clear(); // Clear the dictionary to avoid stale references
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // Implementation needed
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }
    private void Start()
    {
        StartGame(GameMode.AutoHostOrClient);
    }
    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.GetComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Use a unique session name for testing
        string sessionName = "TestRoom_";
        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),

        });

        Debug.Log($"Started game with session name: {sessionName}");
    }
    // private void OnGUI()
    // {
    //    if (_runner == null)
    //  {
    //    if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
    //  {
    //    StartGame(GameMode.Host);
    //}
    //if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
    //{
    //   StartGame(GameMode.Client);
    //  /}
    //}/
    //}
}