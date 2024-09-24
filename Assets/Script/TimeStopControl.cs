using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopControl : MonoBehaviour {
    public GameObject timeStopAreaObject;

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            timeStopAreaObject.SetActive(!timeStopAreaObject.activeSelf);
        }
    }
}
