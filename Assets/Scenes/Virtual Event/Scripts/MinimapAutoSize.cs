using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapAutoSize : MonoBehaviour
{
    public RectTransform minimapRect;
    private void FixedUpdate() {
        const float defaultSize = 240;
        const float maxCoverage = 0.35f;
        float w = Screen.width;
        float h = Screen.height;
        float maxSize = h*maxCoverage;

        if(defaultSize/h >= maxCoverage) {
            minimapRect.sizeDelta = new Vector2(maxSize, maxSize);
        } else {
            minimapRect.sizeDelta = new Vector2(defaultSize, defaultSize);
        }
    }
    // }
    // private void FixedUpdate() {
        // float w = Screen.width;
        // float h = Screen.height;
        // float min = Mathf.Min(w,h);
        // cam.rect = new Rect(1 - 240/w, 1 - 240/h, 200/w, 200/h);
    // }
}
