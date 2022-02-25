using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlookChangeHandler : MonoBehaviour
{
    public void OnAvatarOutlookChanged(AvatarOutlook oldOutlook, AvatarOutlook newOutlook) {
        // [Noah]
        // do anything here to update the player to look as it should
        // ...
        // ...
        Debug.Log(JsonUtility.ToJson(newOutlook));
    }
}
