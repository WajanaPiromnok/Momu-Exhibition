using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform cameraT;
    public List<Transform> trans;
    public float speed;
    float c;
    public float angularSpeed;
    public Transform destination;
    public Transform lookDestination;

    private void LateUpdate()
    {
        if(destination == null && MainNetworkPlayer.Main != null) {
            destination = MainNetworkPlayer.Main.gameObject.GetComponentInChildren<CameraRootForAvatarUI>().transform;
            lookDestination = MainNetworkPlayer.Main.gameObject.GetComponentInChildren<CameraLookForAvatarUI>().transform;
        }

        // if (Vector3.Distance(cameraT.position, destination.position) > 0.001f)
        // {
        //     c += speed * Time.deltaTime;
        //     // cameraT.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(destination.rotation.eulerAngles), angularSpeed * Time.deltaTime);
        // }
        // else
        //     c = 0;
        cameraT.position = destination.position;// Vector3.Lerp(cameraT.position, destination.position, c);
        cameraT.LookAt(lookDestination);
    }
}
