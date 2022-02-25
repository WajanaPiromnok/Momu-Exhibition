using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    public float speed = 60;
    private void Update() {
        transform.Rotate(Vector3.up*speed*Time.deltaTime, Space.Self);    
    }
}
