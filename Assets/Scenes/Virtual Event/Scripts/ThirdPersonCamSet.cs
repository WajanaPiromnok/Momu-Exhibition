using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SmoothFollower))]
public class ThirdPersonCamSet : MonoBehaviour
{
    private SmoothFollower follower;
    private CameraPivot pivot;
    private void Awake() {
        follower = GetComponent<SmoothFollower>();
        pivot = GetComponentInChildren<CameraPivot>();
    }

    public void SetTarget(Transform targetTrans){
        follower.targetTransform = targetTrans;
        pivot.EnableControl();
    }
}
