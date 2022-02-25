using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLookAt : MonoBehaviour
{
    public Transform target;
    public float minY = -0.4f;
    public float maxY = 1.2f;
    public float swingSpeed = 30;
    private float x = 0;
    private void Update() {
        x += Time.deltaTime*swingSpeed;
        Vector3 localPos = transform.localPosition;
        localPos.y = (Mathf.Sin(x)/2+0.5f)*(maxY - minY) + minY;
        transform.localPosition = localPos;
        transform.LookAt(target);    
    }
}
