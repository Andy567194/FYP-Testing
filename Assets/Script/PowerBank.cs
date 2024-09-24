using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBank : MonoBehaviour {
    public Text powerBankText;
    private float power = 0;
    // Start is called before the first frame update
    void Start() {
        powerBankText.text = "Power Bank: " + power;
    }

    // Update is called once per frame
    void Update() {

    }

    public void AddPower(Vector3 storedForce) {
        float temp = Mathf.Abs(storedForce.x) + Mathf.Abs(storedForce.y) + Mathf.Abs(storedForce.z);
        power += temp;
        powerBankText.text = "Power Bank: " + power;
    }

    public void UsePower(float amount) {
        power -= amount;
        powerBankText.text = "Power Bank: " + power;
    }
}
