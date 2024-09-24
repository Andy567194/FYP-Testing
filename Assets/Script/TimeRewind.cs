using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRewind : MonoBehaviour {
    private Rigidbody rb;
    private List<TransformData> transformData; // Store position and rotation
    private float recordTime = 500f; // Time to record positions
    private float recordInterval = 0.1f; // How often to record positions
    private bool isRewinding = false;
    private float cooldown = 0;

    // Struct to hold position and rotation
    private struct TransformData {
        public Vector3 position;
        public Quaternion rotation;

        public TransformData(Vector3 pos, Quaternion rot) {
            position = pos;
            rotation = rot;
        }
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
        transformData = new List<TransformData>();
        StartCoroutine(RecordTransform());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) // Press R to rewind
        {
            isRewinding = true;
        }
        cooldown -= Time.deltaTime;
        if (isRewinding && cooldown <= 0) {
            rb.isKinematic = true;
            Rewind();
            cooldown = 0.03f;
        }
    }

    private IEnumerator RecordTransform() {
        while (true) {
            if (!isRewinding) {
                // Store both position and rotation
                transformData.Add(new TransformData(transform.position, transform.rotation));
                if (transformData.Count > (recordTime / recordInterval)) {
                    transformData.RemoveAt(0); // Remove oldest data
                }
            }
            yield return new WaitForSeconds(recordInterval);
        }
    }

    private void Rewind() {
        if (transformData.Count > 0) {
            // Get the last recorded transform data
            TransformData lastTransform = transformData[transformData.Count - 1];
            transform.position = lastTransform.position; // Move to the last recorded position
            transform.rotation = lastTransform.rotation; // Set the last recorded rotation
            transformData.RemoveAt(transformData.Count - 1); // Remove that data from the list
        } else {
            isRewinding = false; // Stop rewinding if there are no positions left
            rb.isKinematic = false;
        }
    }
}