using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
//using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;



public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private float _mouseSensitivity = 10f;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    public Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    //[Networked, Capacity(12)] private NetworkDictionary<PlayerRef, PlayerController> Players => default;

    public static BasicSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
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
        // Create a unique position for the player
        Vector3 spawnPosition = playerSpawnPoint.position;
        NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

        // Assign input authority to the player object
        networkPlayerObject.AssignInputAuthority(player);

        // Keep track of the player avatars for easy access
        _spawnedCharacters.Add(player, networkPlayerObject);

        // Log whether the player has InputAuthority
        Debug.Log($"Player {player} joined. Has InputAuthority: {networkPlayerObject.HasInputAuthority}");

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

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene load completed.");

        // Optionally, reposition players to new spawn points after the new scene loads
        foreach (var player in runner.ActivePlayers)
        {
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject playerObject))
            {
                var newSpawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
                if (newSpawnPoint != null)
                {
                    playerObject.transform.position = newSpawnPoint.transform.position;
                }
            }
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene loading started.");
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

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        // Ensure the host has input authority
        if (mode == GameMode.Host)
        {
            foreach (var player in _runner.ActivePlayers)
            {
                if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
                {
                    networkObject.AssignInputAuthority(player);
                    Debug.Log($"Host player {player} assigned input authority: {networkObject.HasInputAuthority}");
                }
            }
        }
        else if (mode == GameMode.Client)
        {
            // Ensure the client has input authority
            foreach (var player in _runner.ActivePlayers)
            {
                if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
                {
                    networkObject.AssignInputAuthority(player);
                    Debug.Log($"Client player {player} assigned input authority: {networkObject.HasInputAuthority}");
                }
            }
        }
    }

    private int nextSceneIndex = 1; // Set the initial next scene index

    public void RequestLoadNextLevel()
    {
        if (_runner.IsServer)
        {
            nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            SceneRef sceneRef = SceneRef.FromIndex(nextSceneIndex);
            if (sceneRef.IsValid)
            {
                _runner.LoadScene(sceneRef, LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError($"Failed to get a valid SceneRef for scene index {nextSceneIndex}");
            }
        }
    }
    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
}