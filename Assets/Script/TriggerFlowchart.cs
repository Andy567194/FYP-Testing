//Copy Right by Vincent Lam 2021
using UnityEngine;
using Fungus;

public class TriggerFlowchart : MonoBehaviour {

    public Flowchart flowchart;
    public string triggerInBlockName;
    public string triggerOutBlockName;
    Transform playerTransform;
    public enum triggerInOut { TriggerIn, TriggerOut };
    bool talking = false;

    private void Start() {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (!flowchart) {
            flowchart = GameObject.FindObjectOfType<Flowchart>();
        }
    }
    /*
    void OnTriggerEnter(Collider col) {
        if (this.enabled && triggerInBlockName != "") {
            if (col.CompareTag(triggerTag)) {
                bool hasBlock = flowchart.ExecuteIfHasBlock(triggerInBlockName);
                if (!hasBlock)
                    print("Trigger in Block '" + triggerInBlockName + "' does not exit!");
            }
        }
    }

    void OnTriggerExit(Collider col) {
        if (this.enabled && triggerOutBlockName != "") {
            if (col.CompareTag(triggerTag)) {
                bool hasBlock = flowchart.ExecuteIfHasBlock(triggerOutBlockName);
                if (!hasBlock)
                    print("Trigger out Block '" + triggerOutBlockName + "' does not exit!");
            }
        }
    }
    */

    private void Update() {
        if (Vector2.Distance(playerTransform.position, transform.position) <= 1f) {
            if (Input.GetKeyDown(KeyCode.E)) {
                if (this.enabled && triggerInBlockName != "") {
                    bool hasBlock = flowchart.ExecuteIfHasBlock(triggerInBlockName);
                    if (!hasBlock) {
                        print("Trigger in Block '" + triggerInBlockName + "' does not exit!");
                    }
                    talking = true;
                }
            }
        } else if (talking == true) {
            if (this.enabled && triggerOutBlockName != "") {
                bool hasBlock = flowchart.ExecuteIfHasBlock(triggerOutBlockName);
                if (!hasBlock) {
                    print("Trigger out Block '" + triggerOutBlockName + "' does not exit!");
                }
                talking = false;
            }
        }
    }
}
