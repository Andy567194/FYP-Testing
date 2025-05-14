using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;
using System.Collections.Generic;
using Fusion.Sockets;
using System;

public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Transform sessionListContent;
    [SerializeField] private GameObject sessionListItemPrefab;
    [SerializeField] private int gameSceneIndex = 1; // Set to your game scene's build index

    void Start()
    {
        runner = GetComponent<NetworkRunner>();
        runner.AddCallbacks(this);
        runner.JoinSessionLobby(SessionLobby.ClientServer);
        DontDestroyOnLoad(gameObject); // Ensure runner persists across scenes
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        foreach (Transform child in sessionListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var session in sessionList)
        {
            if (session.IsVisible && session.IsOpen)
            {
                GameObject item = Instantiate(sessionListItemPrefab, sessionListContent);
                TMP_Text text = item.GetComponentInChildren<TMP_Text>();
                text.text = $"{session.Name} ({session.PlayerCount}/{session.MaxPlayers})";
                Button joinButton = item.GetComponentInChildren<Button>();
                joinButton.onClick.AddListener(() => JoinRoom(session.Name));
            }
        }
    }

    public async void CreateRoom()
    {
        string playerName = nameInput.text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Player name cannot be empty");
            return;
        }
        PlayerSettings.PlayerName = playerName;

        string sessionName = "Room" + UnityEngine.Random.Range(1000, 9999);
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(gameSceneIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
    }

    public async void JoinRoom(string sessionName)
    {
        string playerName = nameInput.text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Player name cannot be empty");
            return;
        }
        PlayerSettings.PlayerName = playerName;

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(gameSceneIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }
}