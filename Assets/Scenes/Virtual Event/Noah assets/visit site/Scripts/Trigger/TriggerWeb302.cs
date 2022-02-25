using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class TriggerWeb302 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (MainNetworkPlayer.Main.gameObject == other.gameObject)
        {
            PlaygroundMaster.Instance.ShowNpcWebsite302();
        }
    }

    private void OnTriggerExit(Collider exit)
    {
        if (MainNetworkPlayer.Main.gameObject == exit.gameObject)
        {
            PlaygroundMaster.Instance.HideNpcWebsite();
        }
    }
}
