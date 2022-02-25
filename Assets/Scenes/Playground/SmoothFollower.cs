using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollower : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 offset;
    public float followSpeed = 1;

    private void Update(){
        if(targetTransform != null)
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime*followSpeed);
    }

    public void RemoveTarget(){
        targetTransform = null;
    }
}
