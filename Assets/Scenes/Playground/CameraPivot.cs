using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    private Quaternion targetRotation = Quaternion.identity;
    public bool isEnabled = false;
    public float enablingDelay = 0;
    public float lerpSpeed = 1;
    public float scrollLerpSpeed = 1;
    public float sensitivity = 1;
    public float scrollSensitivity = 1;
    public float minDistance = 2.5f;
    public float maxDistance = 6f;
    // public float maxLookDown = 60;
    private Camera cam;
    private float targetDistance = 0;
    // private float targetLookDown = 0;

    // Update is called once per frame
    private IEnumerator WaitAndEnable()
    {
        yield return new WaitForSeconds(enablingDelay);
        isEnabled = true;
    }

    private void Awake() {
        cam = GetComponentInChildren<Camera>();
        targetDistance = cam.transform.localPosition.z * -1;
        // targetLookDown = transform.rotation.eulerAngles.x;
    }

    void Update()
    {
        if(isEnabled) {
            // myQuaternion *= Quaternion.Euler(Vector3.up * 20);
            float diffAngle = Quaternion.Angle(targetRotation, transform.rotation);
            // Debug.Log(diffAngle);
            float additionAngle = 180 * Input.GetAxis("Mouse X") * sensitivity;// * Time.deltaTime;
            if(additionAngle > 0)
                additionAngle = Mathf.Clamp(additionAngle, 0, 179-diffAngle);
            else
                additionAngle = Mathf.Clamp(additionAngle, -179+diffAngle, 0);
            targetRotation *= Quaternion.Euler(Vector3.up * additionAngle);
            // targetRotation.y += 360*Input.GetAxis("Mouse X")*sensitivity*Time.deltaTime;

            targetDistance -= Input.mouseScrollDelta.y * scrollSensitivity;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            
            
        }

        if(transform.rotation != targetRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*lerpSpeed);

        Vector3 camLocalPost = cam.transform.localPosition;
        camLocalPost.z = Mathf.Lerp(camLocalPost.z, targetDistance*-1, Time.deltaTime*scrollLerpSpeed);
        cam.transform.localPosition = camLocalPost;
            // transform.rotation =Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, targetRotation, Time.deltaTime*lerpSpeed));
    }

    public void Reset() {
        targetRotation = Quaternion.Euler(Vector3.zero);
    }

    public void EnableControl(){
        StartCoroutine(WaitAndEnable());
    }

    public void DisableControl(){
        StopAllCoroutines();
        isEnabled = false;
    }
}
