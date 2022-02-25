using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class WebLibCustom : MonoBehaviour
{
    [DllImport("__Internal")] public static extern bool IsMobileBrowser();
    [DllImport("__Internal")] public static extern void ShowAlert(string str);
    [DllImport("__Internal")] public static extern string ReadCookies(string name);
    [DllImport("__Internal")] public static extern string ReadLocalStorage(string name);
    [DllImport("__Internal")] public static extern string ReadRemoteHostURL();
    [DllImport("__Internal")] public static extern void AllowCursorLock();
    [DllImport("__Internal")] public static extern void DisallowCursorLock();
    [DllImport("__Internal")] public static extern void _UnlockCursor();

    public static void UnlockCursor() {
        #if UNITY_WEBGL
        #if !UNITY_EDITOR
        _UnlockCursor();
        #endif
        #endif
    }
    
    public void OnButtonShowAlert() {
        ShowAlert("Hello Welcome");
    }
    public void OnButtonCheckMobile() {
        ShowAlert(IsMobileBrowser().ToString());
    }
    public void OnButtonReadAuthMode() {
        ShowAlert("from cookies: " + ReadCookies("authMode"));
        ShowAlert("from localStorage: " + ReadLocalStorage("authMode"));
    }
    public void OnButtonReadUserId() {
        ShowAlert("from cookies: " + ReadCookies("userId"));
        ShowAlert("from localStorage: " + ReadLocalStorage("userId"));
    }

}
