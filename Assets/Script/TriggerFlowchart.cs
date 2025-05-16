using UnityEngine;
using Fungus;

public class TriggerFlowchart : MonoBehaviour
{

    public Flowchart flowchart;
    public string triggerInBlockName;
    public string triggerOutBlockName;
    public enum triggerInOut { TriggerIn, TriggerOut };
    public float talkDistance = 2f;
    bool entered = false;

    private void Start()
    {
        if (!flowchart)
        {
            flowchart = GameObject.FindObjectOfType<Flowchart>();
        }
    }
    /*
    void OnTriggerEnter(Collider col)
    {
        if (this.enabled && triggerInBlockName != "")
        {
            PlayerMovement player = col.GetComponent<PlayerMovement>();
            if (player != null)
            {
                bool hasBlock = flowchart.ExecuteIfHasBlock(triggerInBlockName);
                if (!hasBlock)
                    print("Trigger in Block '" + triggerInBlockName + "' does not exit!");
            }
        }
        //Debug.Log("entered");
    }

    void OnTriggerExit(Collider col)
    {
        if (this.enabled && triggerOutBlockName != "")
        {
            PlayerMovement player = col.GetComponent<PlayerMovement>();
            if (player != null)
            {
                bool hasBlock = flowchart.ExecuteIfHasBlock(triggerOutBlockName);
                if (!hasBlock)
                    print("Trigger out Block '" + triggerOutBlockName + "' does not exit!");
            }
        }
        //Debug.Log("exit");
    }
    */

    void Update()
    {
        GameObject player = FindNearestPlayer();
        if (player.name != "PlayerPrefab(Clone)" || player.layer != LayerMask.NameToLayer("LocalPlayerModel"))
            return;

        if (Vector3.Distance(transform.position, player.transform.position) <= talkDistance && entered == false)
        {
            if (this.enabled && triggerInBlockName != "")
            {
                bool hasBlock = flowchart.ExecuteIfHasBlock(triggerInBlockName);
                if (!hasBlock)
                {
                    Debug.Log("Trigger in Block '" + triggerInBlockName + "' does not exit!");
                }
                Cursor.lockState = CursorLockMode.None;
                entered = true;
            }

        }

        if (Vector3.Distance(transform.position, player.transform.position) > talkDistance && entered == true)
        {
            if (this.enabled && triggerOutBlockName != "")
            {

                bool hasBlock = flowchart.ExecuteIfHasBlock(triggerOutBlockName);
                if (!hasBlock)
                {
                    Debug.Log("Trigger out Block '" + triggerOutBlockName + "' does not exit!");
                }
                entered = false;
            }
        }
    }

    public void SetEntered(bool value)
    {
        entered = value;
    }

    GameObject FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPlayer = player;
            }
        }

        return nearestPlayer;
    }
}
