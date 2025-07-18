using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class TimeStopAreaSpawner : NetworkBehaviour
{
    public GameObject objectToSpawn; // The prefab to instantiate
    //public GameObject previewObject; // The prefab for the preview
    //public float spawnDistance = 2f; // Distance from the player to spawn the object
    //public float scrollSpeed = 1f;
    //private GameObject currentPreview; // Reference to the current preview object
    private NetworkObject TSA;


    [Networked] public NetworkBool spawned { get; set; } = false;
    //[Networked] private Vector3 previewPosition { get; set; }
    //[Networked] private Quaternion previewRotation { get; set; }
    //[Networked] private Vector3 spawnedPosition { get; set; }
    //[Networked] private Quaternion spawnedRotation { get; set; }
    [Networked]
    public float storedForce { get; set; } = 0;

    private List<TransformData> transformData = new List<TransformData>();

    public AudioClip[] audioClips;
    private struct TransformData
    {
        public Vector3 position;
        //public Quaternion rotation;

        //public TransformData(Vector3 pos, Quaternion rot)
        public TransformData(Vector3 pos)
        {
            position = pos;
            //rotation = rot;
        }
    }

    [Networked]
    bool isRecordingPlayer { get; set; } = false;
    [Networked]
    bool isRewindingPlayer { get; set; } = false;
    [Networked] float recordedTime { get; set; } = 0;
    public Text recordPlayerText;

    public override void FixedUpdateNetwork()
    {
        if (spawned && TSA != null)
        {
            TSA.transform.position = GetComponent<Collider>().bounds.center;
        }
        if (storedForce > 0 && !spawned)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(GetComponentInChildren<Camera>().transform.forward * storedForce, ForceMode.Impulse);
            storedForce = 0;
            Rpc_stopBuffParticle();
        }
        if (isRecordingPlayer)
        {
            RecordPlayer();
        }
        if (isRewindingPlayer)
        {
            RewindPlayer();
        }

    }

    /*
    void UpdatePreview()
    {
        // Calculate the spawn position in front of the camera
        Vector3 spawnPosition = playerCamera.transform.position + playerCamera.transform.forward * spawnDistance;

        // If the preview object doesn't exist, create it
        if (currentPreview == null)
        {
            currentPreview = Instantiate(previewObject, spawnPosition, playerCamera.transform.rotation);
        }
        else
        {
            // Update the position and rotation of the preview object
            currentPreview.transform.position = spawnPosition;
            currentPreview.transform.rotation = playerCamera.transform.rotation;
        }

        // Update networked variables
        previewPosition = spawnPosition;
        previewRotation = playerCamera.transform.rotation;
    }*/

    /*
    public void SetSpawnDistance(float scrollInput)
    {
        spawnDistance += scrollInput * scrollSpeed;
        spawnDistance = Mathf.Max(spawnDistance, 0);
    }*/

    public void SpawnObject()
    {
        if (isRecordingPlayer)
        {
            CancelRecordPlayer();
            return;
        }
        if (!spawned)
        {
            TSA = Runner.Spawn(objectToSpawn, transform.position, Quaternion.identity, Object.InputAuthority);
            spawned = true;
            Rpc_playerSound2();
        }
        else
        {
            if (TSA != null)
            {
                Runner.Despawn(TSA);
                spawned = false;
                Rpc_playerSound3();
            }
        }
    }

    public void AddStoredForce(float force)
    {
        storedForce += force;
        if (storedForce > 0)
        {
            Rpc_playBuffParticle();
        }
    }

    public void RewindObject()
    {
        GameObject selectedObject = GetComponent<SelectObject>().selectedObject;
        if (selectedObject != null)
        {
            if (selectedObject.GetComponent<TimeRewind>() != null)
            {
                selectedObject.GetComponent<TimeRewind>().Rpc_setIsRewinding(true);
            }
            if (selectedObject.GetComponent<RewindBridge>() != null)
            {
                selectedObject.GetComponent<RewindBridge>().RPC_Rewind();
            }
            if (selectedObject.GetComponent<Shooter>() != null)
            {
                selectedObject.GetComponent<Shooter>().Rpc_Rewind();
            }
        }
    }

    void RecordPlayer()
    {
        Transform playerTransform = FindObjectOfType<IsVisible>().gameObject.transform.parent;
        // Store both position and rotation
        transformData.Add(new TransformData(playerTransform.position));
        recordedTime += Runner.DeltaTime;
        recordPlayerText.text = "Recording player... " + recordedTime.ToString("F0") + "s\nPress Q to rewind or press right mouse button to cancel";
        playerTransform.Find("Canvas").Find("RecordingPlayer").GetComponent<Text>().enabled = true;
        playerTransform.Find("Canvas").Find("RecordingPlayer").GetComponent<Text>().text = "Getting Recorded..." + recordedTime.ToString("F0") + "s";
    }

    void RewindPlayer()
    {
        if (transformData.Count > 0)
        {
            Transform playerTransform = FindObjectOfType<IsVisible>().gameObject.transform.parent;
            playerTransform.Find("Canvas").Find("RecordingPlayer").GetComponent<Text>().text = "Getting Rewinded......";
            // Get the last recorded transform data
            TransformData lastTransform = transformData[transformData.Count - 1];
            playerTransform.position = lastTransform.position; // Move to the last recorded position
            //transform.rotation = lastTransform.rotation; // Set the last recorded rotation
            transformData.RemoveAt(transformData.Count - 1); // Remove that data from the list
        }
        else
        {
            isRewindingPlayer = false;
            Transform playerTransform = FindObjectOfType<IsVisible>().gameObject.transform.parent;
            playerTransform.Find("Canvas").Find("RecordingPlayer").GetComponent<Text>().enabled = false;
            Rpc_stopRewindParticle();
        }
    }

    public void ActivateRecordAndRewindPlayer()
    {
        if (!isRecordingPlayer && !isRewindingPlayer)
        {
            isRecordingPlayer = true;
            recordedTime = 0;
            recordPlayerText.enabled = true;
            Rpc_playRecordParticle();

        }
        else if (isRecordingPlayer && recordedTime >= 0.5)
        {
            isRecordingPlayer = false;
            isRewindingPlayer = true;
            recordPlayerText.enabled = false;
            Rpc_stopRecordParticle();
            Rpc_startRewindParticle();
        }
    }

    public void CancelRecordPlayer()
    {
        if (isRecordingPlayer)
        {
            isRecordingPlayer = false;
            recordPlayerText.enabled = false;
            Transform playerTransform = FindObjectOfType<IsVisible>().gameObject.transform.parent;
            playerTransform.Find("Canvas").Find("RecordingPlayer").GetComponent<Text>().enabled = false;
            transformData.Clear();
            Rpc_stopRecordParticle();
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_playBuffParticle()
    {
        GameObject buff = transform.Find("Buff").gameObject;
        buff.SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_stopBuffParticle()
    {
        GameObject buff = transform.Find("Buff").gameObject;
        buff.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_playRecordParticle()
    {
        Transform playerTransform = FindObjectOfType<IsVisible>().gameObject.transform.parent;
        GameObject plexus = playerTransform.Find("Plexus").gameObject;
        plexus.SetActive(true);
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(audioClips[0]);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_stopRecordParticle()
    {
        Transform playerTransform = FindObjectOfType<IsVisible>().gameObject.transform.parent;
        GameObject plexus = playerTransform.Find("Plexus").gameObject;
        plexus.SetActive(false);

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_startRewindParticle()
    {
        Transform playerTransform = FindObjectOfType<IsVisible>().gameObject.transform.parent;
        GameObject rewind = playerTransform.Find("Magic circle").gameObject;
        rewind.SetActive(true);
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(audioClips[1]);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_stopRewindParticle()
    {
        Transform playerTransform = FindObjectOfType<IsVisible>().gameObject.transform.parent;
        GameObject rewind = playerTransform.Find("Magic circle").gameObject;
        rewind.SetActive(false);
    }

    [Rpc]
    void Rpc_playerSound2()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(audioClips[2]);
        }
    }

    [Rpc]
    void Rpc_playerSound3()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(audioClips[3]);
        }
    }
}