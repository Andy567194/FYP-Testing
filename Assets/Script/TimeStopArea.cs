using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopArea : MonoBehaviour {
    private List<TimeControl> timeControllableObjects = new List<TimeControl>();

    void Start() {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = new Material(renderer.material);
        renderer.material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("TimeStoppable")) {
            TimeControl timeControl = other.GetComponent<TimeControl>();
            if (!timeControllableObjects.Contains(timeControl)) {
                timeControllableObjects.Add(timeControl);
            }
            timeControl.timeStopped = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("TimeStoppable")) {
            TimeControl timeControl = other.GetComponent<TimeControl>();
            timeControl.timeStopped = false;
            timeControllableObjects.Remove(timeControl);
        }
    }

    private void OnDisable() {
        foreach (var timeControl in timeControllableObjects) {
            timeControl.timeStopped = false;
        }
        timeControllableObjects.Clear();
    }
}