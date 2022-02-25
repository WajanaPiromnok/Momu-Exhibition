using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AdaptiveSizeCanvas : MonoBehaviour
{
    // CanvasScaler scaler;
    Canvas canvas;
    private void Awake() {
        canvas = GetComponent<Canvas>();
        canvas.scaleFactor = WebLibCustom.ReadDevicePixelRatio();
        // scaler = GetComponent<CanvasScaler>();
        // scaler.dynamicPixelsPerUnit 
        //size factor = devicePixelRatio
        //rect *= size factor
        //margin *= size factor       
    }
}
