using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBall : MonoBehaviour
{
    MeshRenderer rend;

    private void Awake() {
        rend = GetComponent<MeshRenderer>();
    }

    private void Start() {
        StartCoroutine(AutoTurnOff());
    }

    IEnumerator AutoTurnOff() {
        while(true) {
            yield return new WaitForSeconds(0.2f);
            rend.enabled = false;
        }
    }
}
